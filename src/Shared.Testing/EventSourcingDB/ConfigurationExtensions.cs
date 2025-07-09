using EventSourcing;
using Microsoft.Extensions.Configuration;

namespace Shared.Testing.EventSourcingDB;

public static class ConfigurationExtensions
{
  public static IEnumerable<EventSourcingDbContainer> GetEventSourcingDbContainers(
    this IConfiguration configuration
  ) => configuration
      .GetSection(EventSourcingDbConnections.SectionName)
      .Get<EventSourcingDbConnections>()!
      .Select(c => new EventSourcingDbContainer(c))
      .ToList();
}
