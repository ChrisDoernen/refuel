using System.ComponentModel.DataAnnotations;

namespace EventSourcingDB;

public sealed class EventSourcingDbClientOptions
{
  public const string SectionName = "EventSourcing";
  
  [Required, Url]
  public required string Url { get; set; }

  [Required]
  public required string ApiToken { get; set; }

  [Required]
  public required string Source { get; set; }
}
