namespace EventSourcing;

public class Subject
{
  private readonly List<string> _parts;

  public Subject(string subject)
  {
    if (!subject.StartsWith('/'))
    {
      throw new ArgumentException("Subject must start with '/'", nameof(subject));
    }

    _parts = Split(subject).ToList();
  }

  private Subject(params string[] parts)
  {
    _parts = new List<string>(parts);
  }

  public override string ToString() => string.Join('/', _parts);

  public static Subject Parse(string subject) => new(Split(subject));

  private static string[] Split(string subject)
  {
    return subject.Split('/');
  }

  public static implicit operator string(Subject subject) => subject.ToString();
}
