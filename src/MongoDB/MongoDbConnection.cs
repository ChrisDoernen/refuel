namespace MongoDB;

public sealed class MongoDbConnection
{
  public required string Url { get; set; }

  public required string Database { get; set; }
}
