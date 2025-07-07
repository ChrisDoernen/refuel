using Api.Shared;
using Core.ClubMembership;
using Core.Shared;
using Core.Users;
using MediatR;

namespace Api.ClubMembership;

public class ClubMemberType : ObjectType<ClubMember>
{
  protected override void Configure(IObjectTypeDescriptor<ClubMember> descriptor)
  {
    descriptor.BindFieldsExplicitly();

    descriptor.Field(c => c.Id);
    descriptor.Field(c => c.FirstName);
    descriptor.Field(c => c.LastName);

    descriptor
      .ImplementsNode()
      .ResolveNode<ClubCompoundId>(
        async (context, id) =>
          await context.Service<IMediator>().Send(new GetClubMemberQuery(id.ClubId, id.Id), context.RequestAborted)
      );

    descriptor
      .Field("user")
      .Resolve<User>(
        async (context, cancellationToken) =>
        {
          var id = context.Parent<ClubMember>().Id;
          var query = new GetUserQuery(id);

          return await context.Service<IMediator>().Send(query, cancellationToken);
        }
      );

    descriptor
      .Field("roles")
      .Resolve<IEnumerable<Role>>(
        async (context, cancellationToken) =>
        {
          var ids = context.Parent<ClubMember>().RoleIds;
          var query = new GetRolesQuery();

          var roles = await context.Service<IMediator>().Send(query, cancellationToken);

          return roles.Where(r => ids.Contains(r.Id)).ToList();
        }
      );
  }
}

[ExtendObjectType(typeof(ClubMember))]
public class ClubMemberExtensions
{
  [ID<ClubMember>]
  public ClubCompoundId GetId([Parent] ClubMember member) => new(member.ClubId, member.Id);
}
