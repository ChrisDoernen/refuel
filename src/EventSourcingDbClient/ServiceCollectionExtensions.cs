using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EventSourcingDbClient;

public static class ServiceCollectionExtensions
{
  public static void AddEventSourcingDb(
    this IServiceCollection services,
    Action<EventSourcingDbClientOptions>? configureOptions = null
  )
  {
    services.AddOptions<EventSourcingDbClientOptions>()
      .Configure<IConfiguration>(
        (options, configuration) =>
          configuration.GetSection("EventSourcing").Bind(options)
      );

    if (configureOptions is not null)
    {
      services.PostConfigure(configureOptions);
    }

    services.AddHttpClient(
      "eventsourcingdb",
      (s, client) =>
      {
        var options = s.GetRequiredService<IOptions<EventSourcingDbClientOptions>>().Value;

        client.BaseAddress = new Uri(options.Url);
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {options.ApiToken}");
      }
    );

    services.AddTransient<IEventStore, EventStore>();
    services.AddTransient<IEventSourcingDbClient, EventStore>();
  }
}
