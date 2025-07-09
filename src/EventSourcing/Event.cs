namespace EventSourcing;

/// <summary>
///   This is the same as the event type from EventSourcingDb, but with deserialized data
/// </summary>
public record Event(
  string SpecVersion,
  string Id,
  DateTimeOffset Time,
  string Source,
  string Subject,
  string Type,
  string DataContentType,
  IEventData Data,
  string Hash,
  string PredecessorHash,
  string? TraceParent,
  string? TraceState
);

/// <summary>
///   The event candidate of the public interface.
///   Since the source is always the same, it should be configured for the entire application.
///   The event type is read from the attribute.
/// </summary>
public record EventCandidate(
  string Subject,
  IEventData Data
);
