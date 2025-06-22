using Api.GraphQL;
using Core.Clubs;
using MediatR;

namespace Api.Clubs;

public class ClubsQueryType : ObjectTypeExtension<Query>
{
  protected override void Configure(IObjectTypeDescriptor<Query> descriptor)
  {
    descriptor
      .Field("GetClubs")
      .Resolve<IEnumerable<Club>>(
        async (context, cancellationToken) =>
        {
          var query = new GetClubsQuery();

          return await context.Service<IMediator>().Send(query, cancellationToken);
        }
      );
  }
}
