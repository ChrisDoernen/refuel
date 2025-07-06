namespace EventSourcingDB;

public interface IEventStore
{
  Task Ping(CancellationToken cancellationToken = default);

  Task VerifyApiToken(CancellationToken cancellationToken = default);
  
  Task<IEnumerable<Event>> StoreEvents(
    IEnumerable<EventCandidate> eventCandidates,
    IEnumerable<Precondition>? preconditions = null,
    CancellationToken cancellationToken = default
  );

  Task<IAsyncEnumerable<Event>> GetEvents(
    string subject,
    ReadEventsOptions? options = null,
    CancellationToken cancellationToken = default
  );

  Task<IAsyncEnumerable<EventProjection>> RunEventQlQuery(
    string query,
    CancellationToken cancellationToken = default
  );
}
