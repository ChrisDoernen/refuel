using EventSourcingDb.Types;

namespace EventSourcing;

public interface IEventStore
{
  Task Ping(CancellationToken cancellationToken = default);

  Task VerifyApiToken(CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<Event>> StoreEvents(
    IEnumerable<EventCandidate> eventCandidates,
    IEnumerable<Precondition>? preconditions = null,
    CancellationToken cancellationToken = default
  );

  IAsyncEnumerable<Event> GetEvents(
    Subject subject,
    ReadEventsOptions? readEventsOptions = null,
    CancellationToken cancellationToken = default
  );

  IAsyncEnumerable<Event> ObserveEvents(
    Subject subject,
    ObserveEventsOptions? observeEventsOptions = null,
    CancellationToken cancellationToken = default
  );

  // Task<IAsyncEnumerable<object>> RunEventQlQuery(
  //   string query,
  //   CancellationToken cancellationToken = default
  // );
}
