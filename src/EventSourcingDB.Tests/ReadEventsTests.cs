using FluentAssertions;
using Xunit;
using Xunit.Abstractions;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace EventSourcingDB.Tests;

public class ReadEventsTests(
  ITestOutputHelper testOutputHelper,
  EventSourcingDbFixture fixture
) : TestBed<EventSourcingDbFixture>(testOutputHelper, fixture)
{
  private readonly IEventStore _eventStore = fixture.Get<IEventStore>(testOutputHelper);

  [Fact]
  public async Task ReadEvents()
  {
    var eventData = new TestEventV1(
      Guid.CreateVersion7(),
      "Chris",
      DateTime.UtcNow
    );
    var eventCandidate = new EventCandidate(
      Subject: "/test/42",
      Data: eventData
    );

    await _eventStore.StoreEvents([eventCandidate]);

    var events = await _eventStore.GetEvents("/test/42");

    var eventList = await events.ToListAsync();
    eventList.Count.Should().Be(1);

    var evnt = eventList.First();
    evnt.Data.Should().BeOfType<TestEventV1>();
    evnt.Data.Should().Be(eventData);
  }

  [Fact]
  public async Task ReadEventsRecursively()
  {
    var eventData = new TestEventV1(
      Guid.CreateVersion7(),
      "Chris",
      DateTime.UtcNow
    );
    var testEventCandidate = new EventCandidate(
      Subject: "/test/43/foo",
      Data: eventData
    );
    var otherTestEventCandidate = new EventCandidate(
      Subject: "/test/43/bar",
      Data: new OtherTestEventV1(Guid.CreateVersion7())
    );

    await _eventStore.StoreEvents([testEventCandidate, otherTestEventCandidate]);

    var events = await _eventStore.GetEvents(
      "/test/43",
      new ReadEventsOptions
      {
        Recursive = true
      }
    );

    var eventList = await events.ToListAsync();

    eventList.Count.Should().Be(2);

    eventList.First().Data.Should().BeOfType<TestEventV1>();
    eventList.Last().Data.Should().BeOfType<OtherTestEventV1>();
  }
}
