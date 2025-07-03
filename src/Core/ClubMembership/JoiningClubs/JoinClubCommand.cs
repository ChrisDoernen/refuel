using Core.Users;
using EventSourcingDB;
using MediatR;

namespace Core.ClubMembership.JoiningClubs;

public record JoinClubCommand(
  Guid UserId,
  Guid ClubId
) : IRequest;

public class JoinClubCommandHandler(
  IMediator mediator,
  IEventStoreFactory eventStoreFactory
) : IRequestHandler<JoinClubCommand>
{
  public async Task Handle(
    JoinClubCommand command,
    CancellationToken cancellationToken
  )
  {
    var user = await mediator.Send(new GetUserQuery(command.UserId), cancellationToken);

    var clubJoinedEvent = new UserJoinedClubEventV1(
      user.Id,
      user.FirstName,
      user.FirstName,
      user.LastName
    );
    var candidate = new EventCandidate(
      Subject: $"/member/{command.UserId}",
      Data: clubJoinedEvent
    );
    await eventStoreFactory
      .ForTenant(command.ClubId)
      .StoreEvents(
        [candidate],
        [new IsSubjectPristine(candidate.Subject)],
        cancellationToken
      );
  }
}
