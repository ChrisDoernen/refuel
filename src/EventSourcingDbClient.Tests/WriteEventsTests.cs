using FluentAssertions;
using Xunit;
using Xunit.Abstractions;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace EventSourcingDbClient.Tests;

public class WriteEventsTests(
  ITestOutputHelper testOutputHelper,
  EventSourcingDbFixture fixture
) : TestBed<EventSourcingDbFixture>(testOutputHelper, fixture)
{
  private readonly IEventStore _eventStore = fixture.Get<IEventStore>(testOutputHelper);

  [Fact]
  public async Task WriteSingleEvent()
  {
    var eventCandidate = new EventCandidate(
      Subject: "/test/42",
      Data: new TestEventV1(
        Guid.CreateVersion7(),
        "Chris",
        DateTime.UtcNow
      )
    );

    var events = await _eventStore.StoreEvents(
      [eventCandidate],
      [new IsSubjectPristine(eventCandidate.Subject)]
    );

    events.Count().Should().Be(1);
  }
}
