using Core.Shared;
using Core.Users;
using EventSourcingDB;
using MediatR;

namespace Core.ClubMembership.Joining;

public record JoinClubCommand(
  Guid UserId,
  Guid ClubId
) : IRequest;

public class JoinClubCommandHandler(
  IMediator mediator,
  IEventStoreProvider eventStoreProvider
) : IRequestHandler<JoinClubCommand>
{
  public async Task Handle(
    JoinClubCommand command,
    CancellationToken cancellationToken
  )
  {
    var user = await mediator.Send(new GetUserQuery(command.UserId), cancellationToken);

    var clubJoinedEvent = new UserJoinedClubEventV1(
      ClubId: command.ClubId,
      UserId: user.Id,
      Email: user.Email,
      FirstName: user.FirstName,
      LastName: user.LastName
    );
    var candidate = new EventCandidate(
      Subject: $"/members/{command.UserId}",
      Data: clubJoinedEvent
    );
    await eventStoreProvider
      .ForClub(command.ClubId)
      .StoreEvents(
        [candidate],
        [new IsSubjectPristine(candidate.Subject)],
        cancellationToken
      );
  }
}
