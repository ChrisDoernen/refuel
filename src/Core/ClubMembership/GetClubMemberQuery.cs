using App.Cqrs;
using Core.Shared;
using EventSourcingDb.Types;
using MediatR;

namespace Core.ClubMembership;

public record GetClubMemberQuery(
  Guid ClubId,
  Guid MemberId
) : IRequest<ClubMember>;

public class GetClubMemberQueryHandler(
  IEventStoreProvider eventStoreProvider
) : IRequestHandler<GetClubMemberQuery, ClubMember>
{
  public async Task<ClubMember> Handle(
    GetClubMemberQuery query,
    CancellationToken cancellationToken
  )
  {
    var events = eventStoreProvider
      .ForClub(query.ClubId)
      .GetEvents(
        $"/members/{query.MemberId}",
        new ReadEventsOptions(true),
        cancellationToken
      );

    var changes = events.Select(Change.FromEvent);
    var member = await AuditTrail<ClubMember>.Pristine().Replay(changes);

    return member.EnsureNotPristine().GetAudited();
  }
}
