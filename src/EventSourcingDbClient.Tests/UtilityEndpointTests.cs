using Xunit;
using Xunit.Abstractions;

namespace EventSourcingDbClient.Tests;

public class UtilityEndpointTests(
  ITestOutputHelper testOutputHelper,
  EventSourcingDbFixture fixture
) : IClassFixture<EventSourcingDbFixture>
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
