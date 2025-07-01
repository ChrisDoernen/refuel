namespace EventSourcingDB;

public abstract class Precondition(
  string type,
  object payload
)
{
  public string Type { get; } = type;
  public object Payload { get; } = payload;
}

public class IsSubjectPristine(
  string subject
) : Precondition(
  "isSubjectPristine",
  new { Subject = subject }
);

public class IsSubjectOnEventId(
  string subject,
  string eventId
) : Precondition(
  "isSubjectOnEventId",
  new
  {
    Subject = subject,
    EventId = eventId
  }
);
