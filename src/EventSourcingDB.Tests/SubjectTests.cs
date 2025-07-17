using EventSourcing;
using FluentAssertions;
using Xunit;

namespace EventSourcingDB.Tests;

public class SubjectTests
{
  [Fact]
  public void RunTests()
  {
    var id = Guid.CreateVersion7();
    var subject = new Subject($"/level1/{id}");

    var selectedId = Subject.FromLevel(1)(subject);

    selectedId.Should().Be(id);
  }
}
