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

    descriptor.Field(c => c.UserId);
    descriptor.Field(c => c.FirstName);
    descriptor.Field(c => c.LastName);

    // descriptor
    //   .ImplementsNode()
    //   .ResolveNode<Guid>(
    //     async (context, id) =>
    //       await context.Service<IMediator>().Send(new GetClubMemberQuery(id), context.RequestAborted)
    //   );

    descriptor
      .Field("user")
      .Resolve<User>(
        async (context, cancellationToken) =>
        {
          var id = context.Parent<ClubMember>().UserId;
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
