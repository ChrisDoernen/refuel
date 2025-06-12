namespace EventSourcingDbClient;

public interface IEventData;

public record Event(
  string Type,
  EventPayload Payload
);

public record EventPayload(
  string Source,
  string Subject,
  string Type,
  string SpecVersion,
  string Id,
  DateTime Time,
  string DataContentType,
  string PredecessorHash,
  string Hash,
  IEventData Data
);
