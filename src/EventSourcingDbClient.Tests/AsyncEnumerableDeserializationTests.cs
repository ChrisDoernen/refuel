using System.Text;
using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace EventSourcingDbClient.Tests;

public class AsyncEnumerableDeserializationTests
{
  [Fact]
  public async Task Deserialization()
  {
    const string eventJson =
      """
      {
        "type" : "event",
        "payload" : {
          "source" : "https://example.com",
          "subject" : "/test/42",
          "type" : "com.example.test-event.v1",
          "specversion" : "1.0",
          "id" : "0",
          "time" : "2025-06-09T08:44:56.694264199Z",
          "datacontenttype" : "application/json",
          "predecessorhash" : "0000000000000000000000000000000000000000000000000000000000000000",
          "data" : {
            "_t" : "TestEventV1",
            "id" : "019753dd-39cd-785e-ba47-8492362c4e33"
          },
          "hash" : "98df0b340480f9cf5a2ca44a763677bde02bc262450021d7ae959f8f59b59c5a"
        }
      }
      """;

    var bytes = Encoding.UTF8.GetBytes(eventJson.Replace("\n", "")).ToArray();
    var stream = new MemoryStream(bytes);
    var events = JsonSerializer.DeserializeAsyncEnumerable<Event>(
      stream,
      true,
      JsonSerialization.Options,
      CancellationToken.None
    );

    var eventList = await events.ToListAsync();

    eventList.Count.Should().Be(1);

    var evnt = eventList.First()!;
    evnt.Type.Should().Be("event");
    evnt.Payload.SpecVersion.Should().Be("1.0");
    evnt.Payload.Id.Should().Be("0");
    evnt.Payload.PredecessorHash.Should().Be("0000000000000000000000000000000000000000000000000000000000000000");
    evnt.Payload.Time.Should().BeBefore(DateTime.UtcNow);
    evnt.Payload.Data.Should().BeOfType<TestEventV1>();
  }
}
