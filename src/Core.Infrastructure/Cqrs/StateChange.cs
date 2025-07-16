using EventSourcing;

namespace Core.Infrastructure.Cqrs;

/// <summary>
///   Contains a state together with the event that led to this state.
/// </summary>
public record StateChange<T>(
  Event ProcessedEvent,
  T State
) where T : IReplayable<T>, new();
