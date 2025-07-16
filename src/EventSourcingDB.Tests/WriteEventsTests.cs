using EventSourcing;
using EventSourcingDb.Types;
using FluentAssertions;
using Xunit;

using Xunit.Microsoft.DependencyInjection.Abstracts;
using EventCandidate = EventSourcing.EventCandidate;

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
      Subject: new Subject("/test/42"),
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
        [Precondition.IsSubjectPristinePrecondition(eventCandidate.Subject)]
      );

    events.Count.Should().Be(1);
    events.First().Data.Should().BeOfType<TestEventV1>();
    events.First().Data.As<TestEventV1>().Name.Should().Be("Chris");
  }
}
