using Core.Infrastructure.Cqrs;
using EventSourcing;
using EventSourcingDb.Types;
using MediatR;

namespace Core.ClubMembership;

public record GetClubMemberAuditTrailQuery(
  Guid ClubId,
  Guid MemberId
) : IRequest<AuditTrail<ClubMember>>;

public class GetClubMemberQueryHandler(
  IEventStoreProvider eventStoreProvider
) : IRequestHandler<GetClubMemberAuditTrailQuery, AuditTrail<ClubMember>>
{
  public async Task<AuditTrail<ClubMember>> Handle(
    GetClubMemberAuditTrailQuery query,
    CancellationToken cancellationToken
  )
  {
    var events = eventStoreProvider
      .ForClub(query.ClubId)
      .GetEvents(
        new Subject($"/members/{query.MemberId}"),
        new ReadEventsOptions(true),
        cancellationToken
      );

    var replay = await Replay<ClubMember>.New().ApplyEventStream(events);

    return replay.GetAuditTrail();
  }
}
