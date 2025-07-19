using Api.GraphQL;
using Core.Infrastructure;
using Core.Infrastructure.Roles;
using Core.Roles;
using Core.Roles.Queries;
using MediatR;

namespace Api.Shared;

public class SharedQueryType : ObjectTypeExtension<Query>
{
  protected override void Configure(IObjectTypeDescriptor<Query> descriptor)
  {
    descriptor
      .Field("getRoles")
      .Resolve<IEnumerable<Role>>(
        async (context, cancellationToken) =>
        {
          var query = new GetRolesQuery();

          return await context.Service<IMediator>().Send(query, cancellationToken);
        }
      );
  }
}
