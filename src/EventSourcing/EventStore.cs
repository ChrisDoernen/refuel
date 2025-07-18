using System.Runtime.CompilerServices;
using EventSourcingDb;
using EventSourcingDb.Types;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EventSourcing;

public class EventStore(
  ILogger<IEventStore> logger,
  Client client,
  EventConverter converter,
  IOptions<EventSourcingDbOptions> options
) : IEventStore
{
  public async Task Ping(CancellationToken cancellationToken = default)
  {
    await client.PingAsync(cancellationToken);
  }

  public async Task VerifyApiToken(CancellationToken cancellationToken = default)
  {
    await client.VerifyApiTokenAsync(cancellationToken);
  }

  public async Task<IReadOnlyCollection<Event>> StoreEvents(
    IEnumerable<EventCandidate> eventCandidates,
    IEnumerable<Precondition>? preconditions = null,
    CancellationToken cancellationToken = default
  )
  {
    var candidates = eventCandidates
      .Select(c => new EventSourcingDb.Types.EventCandidate(
          Subject: c.Subject.ToString(),
          Type: EventType.Of(c.Data),
          Data: c.Data,
          Source: options.Value.Source
        )
      );

    var events = await client.WriteEventsAsync(candidates, preconditions, cancellationToken);

    return events.Select(converter.Convert).ToList();
  }

  public async IAsyncEnumerable<Event> GetEvents(
    Subject subject,
    ReadEventsOptions? readEventsOptions = null,
    [EnumeratorCancellation] CancellationToken cancellationToken = default
  )
  {
    var events = client.ReadEventsAsync(
      subject.ToString(),
      readEventsOptions ?? new ReadEventsOptions(true),
      cancellationToken
    );

    await foreach (var evnt in events)
    {
      if (evnt.IsEventSet)
      {
        yield return converter.Convert(evnt.Event);
      }

      if (evnt.IsErrorSet)
      {
        logger.LogError("Error reading event");
      }
    }
  }
  
  
  public async IAsyncEnumerable<Event> ObserveEvents(
    Subject subject,
    ObserveEventsOptions? observeEventsOptions = null,
    [EnumeratorCancellation] CancellationToken cancellationToken = default
  )
  {
    var events = client.ObserveEventsAsync(
      subject.ToString(),
      observeEventsOptions ?? new ObserveEventsOptions(true),
      cancellationToken
    );

    await foreach (var evnt in events)
    {
      if (evnt.IsEventSet)
      {
        yield return converter.Convert(evnt.Event);
      }

      if (evnt.IsErrorSet)
      {
        logger.LogError("Error reading event");
      }
    }
  }
}
