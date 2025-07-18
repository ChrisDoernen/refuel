using MediatR;

namespace EventSourcing;

/// <summary>
///   This is the same as the event type from EventSourcingDb, but with deserialized data.
///   It can be published to the MediatR and be handled by the application.
/// </summary>
public record Event : EventMetadata, INotification
{
  public required IEventData Data { get; init; }
}

public record EventMetadata
{
  public required uint Id { get; init; }
  public required DateTimeOffset Time { get; init; }
  public required string Source { get; init; }
  public required Subject Subject { get; init; }
}

/// <summary>
///   The event candidate of the public interface.
///   Since the source is always the same, it should be configured for the entire application.
///   The event type is read from the attribute.
/// </summary>
public record EventCandidate(
  Subject Subject,
  IEventData Data
);
