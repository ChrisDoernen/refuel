namespace EventSourcingDbClient;

public abstract class Precondition(
  string type
)
{
  public string Type = type;
}

public class IsSubjectPristine(
  string subject
) : Precondition("isSubjectPristine")
{
  public object Payload => new
  {
    subject
  };
}

public class IsSubjectOnEventId(
  string subject,
  string eventId
) : Precondition("isSubjectOnEventId")
{
  public object Payload => new
  {
    subject,
    eventId
  };
}
