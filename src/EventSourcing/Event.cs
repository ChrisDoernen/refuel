using MediatR;

namespace EventSourcing;

/// <summary>
///   This is the same as the event type from EventSourcingDb, but with deserialized data.
///   It can be published to the MediatR and be handled by the application.
/// </summary>
public record Event(
  string Id,
  DateTimeOffset Time,
  string Source,
  string Subject,
  IEventData Data
) : INotification;

/// <summary>
///   The event candidate of the public interface.
///   Since the source is always the same, it should be configured for the entire application.
///   The event type is read from the attribute.
/// </summary>
public record EventCandidate(
  string Subject,
  IEventData Data
);
