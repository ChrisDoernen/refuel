using Xunit;
using Xunit.Abstractions;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace EventSourcingDB.Tests;

public class UtilityEndpointTests(
  ITestOutputHelper testOutputHelper,
  EventSourcingDbFixture fixture
) : TestBed<EventSourcingDbFixture>(testOutputHelper, fixture)
{
  private readonly IEventSourcingDbClient _client = fixture.Get<IEventSourcingDbClient>(testOutputHelper);

  [Fact]
  public async Task Ping()
  {
    await _client.Ping();
  }

  [Fact]
  public async Task VerifyApiToken()
  {
    await _client.VerifyApiToken();
  }
}
