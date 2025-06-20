using Core.Shared;
using EventSourcingDB;
using MediatR;

namespace Core.Users;

public record GetUserQuery(
  Guid UserId
) : IRequest<AuditTrail<User>>;

public class GetUserQueryHandler(
  IEventStore eventStore
) : IRequestHandler<GetUserQuery, AuditTrail<User>>
{
  public async Task<AuditTrail<User>> Handle(
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

    return await AuditTrail<User>.Pristine().Replay(changes);
  }
}
