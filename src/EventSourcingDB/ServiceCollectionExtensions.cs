using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventSourcingDB;

public static class ServiceCollectionExtensions
{
  public static void AddEventSourcingDb(
    this IServiceCollection services,
    IConfiguration configuration,
    Action<IList<EventSourcingDbConnection>>? configureConnections = null
  )
  {
    services
      .AddOptions<EventSourcingDbOptions>()
      .Bind(configuration.GetSection(EventSourcingDbOptions.SectionName))
      .ValidateOnStart();

    var connections =
      configuration
        .GetSection("EventSourcingDb:Connections")
        .Get<IList<EventSourcingDbConnection>>()
      ?? throw new InvalidOperationException("EventSourcingDb connections are not configured.");

    configureConnections?.Invoke(connections);

    foreach (var connection in connections)
    {
      services.AddHttpClient(
        $"eventsourcingdb-{connection.TenantId}",
        client =>
        {
          client.BaseAddress = new UriBuilder(connection.Url) { Path = "api/v1/" }.Uri;
          client.DefaultRequestHeaders.Add("Authorization", $"Bearer {connection.ApiToken}");
        }
      );
    }

    services.AddTransient<IEventStore, EventStore>();
    services.AddTransient<IEventSourcingDbClient, EventStore>();
  }
}
