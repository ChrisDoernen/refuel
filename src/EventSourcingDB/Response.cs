using System.Text.Json.Serialization;

namespace EventSourcingDB;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(EventResponse), typeDiscriminator: "event")]
[JsonDerivedType(typeof(ErrorResponse), typeDiscriminator: "error")]
[JsonDerivedType(typeof(ProjectionResponse), typeDiscriminator: "row")]
public abstract class Response
{
  protected string Type { get; set; } = null!;
}

public class ProjectionResponse : Response
{
  public EventProjection Payload { get; set; } = null!;
}

public class EventResponse : Response
{
  public Event Payload { get; set; } = null!;
}

public class ErrorResponse : Response
{
  public Error Payload { get; set; } = null!;
}

/// <summary>
///   This is used when projecting "in e" in GraphQL.
/// </summary>
public class EventProjection
{
  public string Source { get; set; } = null!;
  public string Subject { get; set; } = null!;
  public string Type { get; set; } = null!;
  public string Id { get; set; } = null!;
  public DateTime Time { get; set; }
  public IEventData Data { get; set; } = null!;
}

public class Event
{
  public string Source { get; set; } = null!;
  public string Subject { get; set; } = null!;
  public string Type { get; set; } = null!;
  public string Id { get; set; } = null!;
  public DateTime Time { get; set; }
  public IEventData Data { get; set; } = null!;
  public string DataContentType { get; set; } = null!;
  public string SpecVersion { get; set; } = null!;
  public string PredecessorHash { get; set; } = null!;
  public string Hash { get; set; } = null!;
}

public interface IEventData;

public class Error
{
  public string Message { get; set; } = null!;
}
