using EventSourcing;
using EventSourcingDb.Types;

namespace Core.Infrastructure.Cqrs;

public interface IAuditTrailReplayService<T> where T : IReplayable<T>, new()
{
  Task<AuditTrail<T>> GetAuditTrail(
    Guid clubId,
    Subject subject,
    CancellationToken cancellationToken
  );
}

public class AuditTrailReplayService<T>(
  IEventStoreProvider eventStoreProvider
) : IAuditTrailReplayService<T> where T : IReplayable<T>, new()
{
  public async Task<AuditTrail<T>> GetAuditTrail(
    Guid clubId,
    Subject subject,
    CancellationToken cancellationToken
  )
  {
    var events = eventStoreProvider
      .ForClub(clubId)
      .GetEvents(
        subject,
        new ReadEventsOptions(true),
        cancellationToken
      );

    var replay = await Replay<T>.New().ApplyEventStream(events);

    return replay.GetAuditTrail();
  }
}
