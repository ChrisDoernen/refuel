namespace EventSourcingDB;

public class ReadEventsOptions
{
  public bool Recursive { get; init; }
  public Order? Order { get; init; }
  public Bound? LowerBound { get; init; }
  public Bound? UpperBound { get; init; }
  public ReadFromLatestEvent? FromLatestEvent { get; init; }
}

public class ReadFromLatestEvent
{
  public required string Subject { get; init; }
  public required string Type { get; init; }
  public ReadIfEventIsMissing IfEventIsMissing { get; init; }
}

public enum Order
{
    Chronological,
    Antichronological
}

public enum ReadIfEventIsMissing
{
  ReadNothing,
  ReadEverything
}

