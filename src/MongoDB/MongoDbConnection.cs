namespace MongoDB;

public sealed class MongoDbConnection
{
  public const string SectionName = "MongoDb";

  public required string Url { get; set; }

  public required string Database { get; set; }
}
