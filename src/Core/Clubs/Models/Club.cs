using MongoDB;

namespace Core.Clubs.Models;

public class Club(
  string name,
  string tenantId,
  string? description
) : Document
{
  public string Name { get; private set; } = name;
  public string TenantId { get; private set; } = tenantId;
  public string? Description { get; private set; } = description;
}
