namespace EventSourcing;

public class Subject
{
  private readonly List<string> _levels;

  public Subject(string subject)
  {
    if (!subject.StartsWith('/'))
    {
      throw new ArgumentException("Subject must start with '/'", nameof(subject));
    }

    _levels = Split(subject).ToList();
  }

  private Subject(params string[] parts)
  {
    _levels = new List<string>(parts);
  }

  public override string ToString() => "/" + string.Join('/', _levels);

  public static Subject Parse(string subject) => new(Split(subject));

  private static string[] Split(string subject)
  {
    return subject.Split('/', StringSplitOptions.RemoveEmptyEntries);
  }

  public static Func<Subject, Guid> FromLevel(int index)
  {
    return subject => subject.IdFromLevel(index);
  }

  private Guid IdFromLevel(int index)
  {
    var level = _levels[index];

    return Guid.Parse(level);
  }

  public static implicit operator string(Subject subject) => subject.ToString();
}
