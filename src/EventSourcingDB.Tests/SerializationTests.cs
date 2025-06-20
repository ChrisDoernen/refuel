using System.Text;
using System.Text.Json;
using FluentAssertions;
using Shared.Testing;
using Xunit;

namespace EventSourcingDB.Tests;

public class SerializationTests
{
  [Fact]
  public async Task DeserializeResponses()
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
      {
        "type" : "error",
        "payload" : {
          "message" : "Ohno!"
        }
      }
      """;

    var bytes = Encoding.UTF8.GetBytes(eventJson.Minify()).ToArray();
    var stream = new MemoryStream(bytes);
    var events = JsonSerializer.DeserializeAsyncEnumerable<Response>(
      stream,
      true,
      JsonSerialization.Options,
      CancellationToken.None
    );

    var eventList = await events.ToListAsync();

    eventList.Count.Should().Be(2);

    eventList.First().Should().BeOfType<EventResponse>();

    var evnt = (EventResponse)eventList.First()!;
    evnt.Payload.SpecVersion.Should().Be("1.0");
    evnt.Payload.Id.Should().Be("0");
    evnt.Payload.PredecessorHash.Should().Be("0000000000000000000000000000000000000000000000000000000000000000");
    evnt.Payload.Time.Should().BeBefore(DateTime.UtcNow);
    evnt.Payload.Data.Should().BeOfType<TestEventV1>();
    
    eventList.Last().Should().BeOfType<ErrorResponse>();

    var error = (ErrorResponse)eventList.Last()!;
    error.Payload.Message.Should().Be("Ohno!");
  }


  [Fact]
  public void Preconditions()
  {
    Precondition isSubjectPristine = new IsSubjectPristine("/test/42");
    var preconditionJson = JsonSerializer.Serialize(
      isSubjectPristine,
      JsonSerialization.Options
    );

    var expectedJson =
      """
        {
          "payload": {
            "subject": "/test/42"
          },
          "type": "isSubjectPristine"
        }
        """.Minify();

    preconditionJson.Should().Be(expectedJson);
  }
}
