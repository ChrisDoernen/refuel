using FluentAssertions;
using Xunit;

using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace EventSourcingDB.Tests;

public class WriteEventsTests(
  ITestOutputHelper testOutputHelper,
  EventSourcingDbFixture fixture
) : TestBed<EventSourcingDbFixture>(testOutputHelper, fixture)
{
  private readonly IEventStoreFactory _eventStoreFactory = fixture.Get<IEventStoreFactory>(testOutputHelper);

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

    var events = await _eventStoreFactory
      .ForTenant("tenant1")
      .StoreEvents(
        [eventCandidate],
        [new IsSubjectPristine(eventCandidate.Subject)]
      );

    events.Count().Should().Be(1);
  }
}
