using Api.GraphQL;
using App;
using Core.Shared;
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
