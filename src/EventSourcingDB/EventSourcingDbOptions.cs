using System.ComponentModel.DataAnnotations;

namespace EventSourcingDB;

public sealed class EventSourcingDbOptions
{
  public const string SectionName = "EventSourcingDb";

  [Required]
  public required string Source { get; set; }
}

public sealed class EventSourcingDbConnections : List<EventSourcingDbConnection>
{
  public const string SectionName = "EventSourcingDb:Connections";

  public EventSourcingDbConnection ForTenant(string tenantId)
    => this.SingleOrDefault(c => c.TenantId == tenantId)
       ?? throw new InvalidOperationException($"No connection found for tenant '{tenantId}'.");
}

public sealed class EventSourcingDbConnection
{
  [Required]
  public required string TenantId { get; set; }

  [Required, Url]
  public required string Url { get; set; }

  [Required]
  public required string ApiToken { get; set; }
}
