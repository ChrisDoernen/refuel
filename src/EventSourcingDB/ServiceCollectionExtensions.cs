using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EventSourcingDB;

public static class ServiceCollectionExtensions
{
  public static void AddEventSourcingDb(
    this IServiceCollection services,
    IConfiguration configuration,
    Action<EventSourcingDbConnections>? configureConnections = null
  )
  {
    services
      .AddOptions<EventSourcingDbOptions>()
      .Bind(configuration.GetSection(EventSourcingDbOptions.SectionName))
      .ValidateOnStart();

    services
      .AddOptions<EventSourcingDbConnections>()
      .Bind(configuration.GetSection(EventSourcingDbConnections.SectionName))
      .ValidateOnStart();

    var connections =
      configuration
        .GetSection(EventSourcingDbConnections.SectionName)
        .Get<EventSourcingDbConnections>()
      ?? throw new InvalidOperationException("EventSourcingDb connections are not configured.");

    if (configureConnections is not null)
    {
      services.PostConfigure(configureConnections);
    }

    foreach (var connection in connections)
    {
      services.AddHttpClient(
        $"eventsourcingdb-{connection.TenantId}",
        (sp, client) =>
        {
          var configuredConnections = sp.GetRequiredService<IOptions<EventSourcingDbConnections>>().Value;
          var configuredConnection = configuredConnections.ForTenant(connection.TenantId);

          client.BaseAddress = new UriBuilder(configuredConnection.Url) { Path = "api/v1/" }.Uri;
          client.DefaultRequestHeaders.Add("Authorization", $"Bearer {configuredConnection.ApiToken}");
        }
      );
    }

    services.AddTransient<IEventStoreFactory, EventStoreFactory>();
  }
}
