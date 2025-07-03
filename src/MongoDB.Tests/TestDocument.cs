namespace MongoDB.Tests;

public record TestDocument : IDocument
{
  public string Property { get; init; } = string.Empty;
  public DateTime Timestamp { get; init; }
}
