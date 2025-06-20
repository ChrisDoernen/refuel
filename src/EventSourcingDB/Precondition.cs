namespace EventSourcingDB;

public abstract class Precondition(
  string type
)
{
  public string Type => type;
}

public class IsSubjectPristine(
  string subject
) : Precondition("isSubjectPristine")
{
  public object Payload => new
  {
    Subject = subject
  };
}

public class IsSubjectOnEventId(
  string subject,
  string eventId
) : Precondition("isSubjectOnEventId")
{
  public object Payload => new
  {
    Subject = subject,
    EventId = eventId
  };
}
