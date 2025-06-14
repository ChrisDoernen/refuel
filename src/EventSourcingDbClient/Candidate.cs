namespace EventSourcingDbClient;

/// <summary>
///   The event candidate of the public interface.
///   Since the source is always the same, it should be configured for the entire application.
///   The event type is read from the attribute.
/// </summary>
public record EventCandidate(
  string Subject,
  IEventData Data
);

internal record Candidate(
  string Source,
  string Subject,
  string Type,
  object Data
);
