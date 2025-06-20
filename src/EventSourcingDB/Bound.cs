namespace EventSourcingDB;

public class Bound
{
  public required string Id { get; init; }
  public BoundType Type { get; set; }
}

public enum BoundType
{
  Inclusive,
  Exclusive
}
