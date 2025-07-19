using Api.GraphQL;
using Core.Clubs;
using Core.Clubs.Models;
using Core.Clubs.Queries;
using MediatR;

namespace Api.Clubs;

public class ClubsQueryType : ObjectTypeExtension<Query>
{
  protected override void Configure(IObjectTypeDescriptor<Query> descriptor)
  {
    descriptor
      .Field("getClubs")
      .Resolve<IEnumerable<Club>>(
        async (context, cancellationToken) =>
          await context.Service<IMediator>().Send(new GetClubsQuery(), cancellationToken)
      );
  }
}
