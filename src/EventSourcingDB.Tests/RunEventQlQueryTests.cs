using FluentAssertions;
using Xunit;
using Xunit.Abstractions;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace EventSourcingDB.Tests;

public class RunEventQlQueryTests(
  ITestOutputHelper testOutputHelper,
  EventSourcingDbFixture fixture
) : TestBed<EventSourcingDbFixture>(testOutputHelper, fixture)
{
  private readonly IEventStore _eventStore = fixture.Get<IEventStore>(testOutputHelper);

  [Fact]
  public async Task EmptyEvents()
  {
    var events = await _eventStore.RunEventQlQuery("FROM e IN events PROJECT INTO e");

    (await events.ToListAsync()).Should().BeEmpty();
  }

  [Fact]
  public async Task GetEventTypes()
  {
    var testEvent = new TestEventV1(
      Guid.CreateVersion7(),
      "Chris",
      DateTime.UtcNow
    );
    var eventCandidate = new EventCandidate(
      Subject: "/test/42",
      Data: testEvent
    );
    var otherTestEvent = new OtherTestEventV1(Guid.CreateVersion7());
    var otherEventCandidate = new EventCandidate(
      Subject: "/other",
      Data: otherTestEvent
    );

    await _eventStore.StoreEvents([eventCandidate, otherEventCandidate]);

    var query =
      $"""
       FROM e IN events
       WHERE e.type == '{EventType.Of(testEvent)}' AND e.data.id == '{testEvent.Id}'
       PROJECT INTO e
       """;

    var events = await _eventStore.RunEventQlQuery(query);

    var eventList = await events.ToListAsync();
    eventList.Count.Should().Be(1);
    eventList.First().Data.Should().BeOfType<TestEventV1>();

    var testEventResult = (TestEventV1)eventList.First().Data;

    testEventResult.Name.Should().Be(testEvent.Name);
    testEventResult.When.Should().Be(testEvent.When);
  }
}
