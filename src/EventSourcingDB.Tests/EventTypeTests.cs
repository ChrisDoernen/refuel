using FluentAssertions;
using Xunit;

namespace EventSourcingDB.Tests;

public class EventTypeTests
{
  [Fact]
  public void GetEventTypeOfGenericType()
  {
    EventType.Of<TestEventV1>().Should().Be("com.example.test-event.v1");
  }
}
