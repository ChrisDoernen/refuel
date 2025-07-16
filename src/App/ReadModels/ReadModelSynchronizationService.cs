using App.Cqrs;
using EventSourcing;
using Microsoft.Extensions.Logging;

namespace App.ReadModels;

public interface IReadModelSynchronizationService
{
  Task Replay(Event evnt, CancellationToken cancellationToken);
}

/// <summary>
///   Synchronizes the read model with events, usually replayed from the event store.
/// </summary>
public class ReadModelSynchronizationService<T>(
  IReadModelRepository<T> repository,
  IEnumerable<Type> relevantEventTypes,
  Func<Subject, Guid> getIdFromSubject
) : IReadModelSynchronizationService where T : IReplayable<T>, new()
{
  public async Task Replay(Event evnt, CancellationToken cancellationToken)
  {
    // Discard events not relevant for this read model
    // ToDo: Automated way to determine interesting events with reflection to reduce manual errors?
    if (!relevantEventTypes.Contains(evnt.GetType()))
    {
      return;
    }

    // Id is extracted from the subject. Most of the time the subject is "/something/id"
    var id = getIdFromSubject(evnt.Subject);

    var lastStateChange = await repository.GetById(id, cancellationToken);

    // We are working with an existing read model
    if (lastStateChange is not null)
    {
      // Skip events that have already been processed
      if (!(evnt.Id > lastStateChange.ProcessedEvent.Id))
      {
        return;
      }

      var updatedReadModel = lastStateChange.State.Apply(evnt.Data);
      var stateChange = new StateChange<T>(evnt, updatedReadModel);

      await repository.Update(stateChange, cancellationToken);
    }

    // Read model does not exist yet.
    // We can create it if the event initializes a subject
    if (lastStateChange is null && evnt.Data is IInitializesSubject)
    {
      var newReadModel = new T().Apply(evnt.Data);
      var newStateChange = new StateChange<T>(evnt, newReadModel);

      await repository.Create(newStateChange, cancellationToken);
    }
    else
    {
      throw new Exception("Can not replay from the middle of a read model lifecycle");
    }
  }
}
