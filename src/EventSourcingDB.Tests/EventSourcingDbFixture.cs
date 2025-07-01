using dotenv.net;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shared.Testing;
using Xunit;
using Xunit.Abstractions;
using Xunit.Microsoft.DependencyInjection;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace EventSourcingDB.Tests;

public class EventSourcingDbFixture : TestBedFixture, IAsyncLifetime
{
  private readonly EventSourcingDbContainer _eventSourcingDbTestcontainer;

  public EventSourcingDbFixture(IMessageSink _)
  {
    DotEnv.Load(new DotEnvOptions(probeForEnv: true, probeLevelsToSearch: 6));

    var options = Configuration!
      .GetSection(EventSourcingDbOptions.SectionName)
      .Get<EventSourcingDbOptions>()!;

    _eventSourcingDbTestcontainer = new EventSourcingDbContainer(options);
  }

  public async Task InitializeAsync()
  {
    await _eventSourcingDbTestcontainer.Start();
  }

  public T Get<T>(ITestOutputHelper testOutputHelper)
    => GetService<T>(testOutputHelper) ?? throw new Exception($"Service missing: {typeof(T).Name}");

  protected override void AddServices(
    IServiceCollection services,
    IConfiguration? configuration
  )
  {
    services.AddEventSourcingDb(
      configuration!,
      _eventSourcingDbTestcontainer?.ConfigureOptions
    );
  }

  protected override IEnumerable<TestAppSettings> GetTestAppSettings()
  {
    yield return new TestAppSettings { Filename = "appsettings.json", IsOptional = false };
  }

  protected override async ValueTask DisposeAsyncCore()
  {
    if (_eventSourcingDbTestcontainer != null)
    {
      await _eventSourcingDbTestcontainer.DisposeAsync();
    }
  }

  public new async Task DisposeAsync() => await base.DisposeAsync();
}
