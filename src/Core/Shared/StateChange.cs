using EventSourcing;

namespace Core.Shared;

public record StateChange<T>(
  Change Change,
  T State
);

public record Change(
  string Id,
  string Subject,
  DateTimeOffset Time,
  IEventData Data
)
{
  public static Change FromEvent(Event evnt) =>
    new(
      evnt.Id,
      evnt.Subject,
      evnt.Time,
      evnt.Data
    );
}
