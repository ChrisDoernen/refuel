using EventSourcingDb;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EventSourcing;

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
      services.AddKeyedTransient<Client>(
        $"esdbclient-{connection.TenantId}",
        (sp, _) =>
        {
          var configuredConnections = sp.GetRequiredService<IOptions<EventSourcingDbConnections>>().Value;
          var configuredConnection = configuredConnections.ForTenant(connection.TenantId);
          var logger = sp.GetRequiredService<ILogger<Client>>();

          var uri = new Uri(configuredConnection.Url);

          return new Client(uri, configuredConnection.ApiToken, logger);
        }
      );
    }

    services.AddTransient<IEventStoreFactory, EventStoreFactory>();
    services.AddSingleton<EventConverter>();
  }
}
