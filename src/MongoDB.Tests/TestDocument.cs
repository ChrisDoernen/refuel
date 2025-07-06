namespace MongoDB.Tests;

public class TestDocument : Document
{
  public string Property { get; init; } = string.Empty;
  public DateTime Timestamp { get; init; }
}
