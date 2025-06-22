using Api.GraphQL;
using Core.Users;
using MediatR;

namespace Api.Users;

public class UsersQueryType : ObjectTypeExtension<Query>
{
  protected override void Configure(IObjectTypeDescriptor<Query> descriptor)
  {
    descriptor
      .Field("getUsers")
      .Resolve<IEnumerable<User>>(
        async (context, cancellationToken) =>
        {
          var query = new GetUsersQuery();

          return await context.Service<IMediator>().Send(query, cancellationToken);
        }
      );
  }
}
