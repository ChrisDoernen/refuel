using Core.Clubs;
using Core.Users;
using MediatR;

namespace Api.Users;

public class UsersQueryType : ObjectType<User>
{
  protected override void Configure(IObjectTypeDescriptor<User> descriptor)
  {
    descriptor
      .Field("GetUsers")
      .Resolve<IEnumerable<User>>(
        async (context, cancellationToken) =>
        {
          var query = new GetUsersQuery();

          return await context.Service<IMediator>().Send(query, cancellationToken);
        }
      );
  }
}
