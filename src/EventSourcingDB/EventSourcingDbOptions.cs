using System.ComponentModel.DataAnnotations;

namespace EventSourcingDB;

public sealed class EventSourcingDbOptions
{
  public const string SectionName = "EventSourcingDb";

  [Required]
  public required string Source { get; set; }
}

public sealed class EventSourcingDbConnection
{
  public required string TenantId { get; set; }

  public required string Url { get; set; }

  public required string ApiToken { get; set; }
}
