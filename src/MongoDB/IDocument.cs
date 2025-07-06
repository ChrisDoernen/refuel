namespace MongoDB;

public interface IDocument
{
  public Guid Id { get; set; }
}

public class Document(Guid? id = null) : IDocument
{
  public Guid Id { get; set; } = id ?? Guid.CreateVersion7();
}
