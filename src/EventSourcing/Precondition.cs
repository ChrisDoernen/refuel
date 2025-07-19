using System.Globalization;

namespace EventSourcing;

public abstract record Precondition
{
  public abstract EventSourcingDb.Types.Precondition Map();
}

public record IsSubjectPristinePrecondition(
  Subject Subject
) : Precondition
{
  public override EventSourcingDb.Types.Precondition Map()
    => EventSourcingDb.Types.Precondition.IsSubjectPristinePrecondition(Subject.ToString());
}

public record IsSubjectOnEventIdPrecondition(
  Subject Subject,
  uint EventId
) : Precondition
{
  public override EventSourcingDb.Types.Precondition Map()
    => EventSourcingDb.Types.Precondition.IsSubjectOnEventIdPrecondition(
      Subject.ToString(),
      EventId.ToString(CultureInfo.InvariantCulture)
    );
}
