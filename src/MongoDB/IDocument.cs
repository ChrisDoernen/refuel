namespace MongoDB;

public record IDocument
{
  public Guid Id { get; }
}

public record Document : IDocument
{
  public Guid Id { get; } = Guid.CreateVersion7();
}
