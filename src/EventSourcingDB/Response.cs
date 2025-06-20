using System.Text.Json.Serialization;

namespace EventSourcingDB;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(EventResponse), typeDiscriminator: "event")]
[JsonDerivedType(typeof(ErrorResponse), typeDiscriminator: "error")]
public abstract class Response
{
  protected string Type { get; set; } = null!;
}

public class EventResponse : Response
{
  public Event Payload { get; set; } = null!;
}

public class ErrorResponse : Response
{
  public Error Payload { get; set; } = null!;
}

public record Event(
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

public interface IEventData;

public record Error(
  string Message
);
