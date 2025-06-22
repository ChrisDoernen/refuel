using Core.Shared;
using EventSourcingDB;
using MediatR;

namespace Core.Users;

public record GetUserQuery(
  Guid UserId
) : IRequest<User>;

public class GetUserQueryHandler(
  IEventStore eventStore
) : IRequestHandler<GetUserQuery, User>
{
  public async Task<User> Handle(
    GetUserQuery query,
    CancellationToken cancellationToken
  )
  {
    var events = await eventStore.GetEvents(
      $"/users/{query.UserId}",
      new ReadEventsOptions
      {
        Recursive = true
      },
      cancellationToken
    );

    var changes = events.Select(Change.FromEvent);
    var user = await AuditTrail<User>.Pristine().Replay(changes);

    return user.GetAudited();
  }
}
