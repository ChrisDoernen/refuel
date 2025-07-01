using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EventSourcingDB;

public static class ServiceCollectionExtensions
{
  public static void AddEventSourcingDb(
    this IServiceCollection services,
    IConfiguration config,
    Action<EventSourcingDbOptions>? configureOptions = null
  )
  {
    services
      .AddOptions<EventSourcingDbOptions>()
      .Bind(config.GetSection(EventSourcingDbOptions.SectionName))
      .ValidateOnStart();

    if (configureOptions is not null)
    {
      services.PostConfigure(configureOptions);
    }

    services.AddHttpClient(
      "eventsourcingdb",
      (s, client) =>
      {
        var options = s.GetRequiredService<IOptions<EventSourcingDbOptions>>().Value;

        client.BaseAddress = new UriBuilder(options.Url) { Path = "api/v1/" }.Uri;
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {options.ApiToken}");
      }
    );

    services.AddTransient<IEventStore, EventStore>();
    services.AddTransient<IEventSourcingDbClient, EventStore>();
  }
}
