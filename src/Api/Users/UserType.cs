using Core.ClubMembership.Projections;
using Core.ClubMembership.Queries;
using Core.Users.Models;
using Core.Users.Queries;
using MediatR;

namespace Api.Users;

public class UserType : ObjectType<User>
{
  protected override void Configure(IObjectTypeDescriptor<User> descriptor)
  {
    descriptor.BindFieldsExplicitly();

    descriptor
      .ImplementsNode()
      .IdField(u => u.Id)
      .ResolveNode(async (context, id) =>
        await context.Service<IMediator>().Send(new GetUserQuery(id), context.RequestAborted)
      );

    descriptor.Field(u => u.FirstName);
    descriptor.Field(u => u.LastName);

    descriptor
      .Field("clubsMembership")
      .Resolve<IEnumerable<ClubMember>>(async (context, cancellationToken) =>
        {
          var userId = context.Parent<User>().Id;
          var query = new GetClubMembershipQuery(userId);

          return await context.Service<IMediator>().Send(query, cancellationToken);
        }
      );
  }
}
