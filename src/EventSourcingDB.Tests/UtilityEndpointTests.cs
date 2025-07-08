using Xunit;

using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace EventSourcingDB.Tests;

public class UtilityEndpointTests(
  ITestOutputHelper testOutputHelper,
  EventSourcingDbFixture fixture
) : TestBed<EventSourcingDbFixture>(testOutputHelper, fixture)
{
  private readonly IEventStoreFactory _eventStoreFactory = fixture.Get<IEventStoreFactory>(testOutputHelper);

  [Fact]
  public async Task Ping()
  {
    await _eventStoreFactory.ForTenant("tenant1").Ping();
  }

  [Fact]
  public async Task VerifyApiToken()
  {
    await _eventStoreFactory.ForTenant("tenant1").VerifyApiToken();
  }
}
