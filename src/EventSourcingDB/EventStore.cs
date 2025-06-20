using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;

namespace EventSourcingDB;

public class EventStore(
  IHttpClientFactory factory
) : IEventStore, IEventSourcingDbClient
{
  private readonly HttpClient _client = factory.CreateClient("eventsourcingdb");

  public async Task Ping(CancellationToken cancellationToken)
  {
    var response = await _client.GetAsync("ping", cancellationToken);
    response.EnsureSuccessStatusCode();

    var evnt = await response.Content.ReadFromJsonAsync<Event>(cancellationToken);
    if (evnt is null || evnt.Type != "io.eventsourcingdb.api.ping-received")
    {
      throw new HttpRequestException("Failed to ping");
    }
  }

  public async Task VerifyApiToken(CancellationToken cancellationToken)
  {
    var response = await _client.PostAsync(
      "verify-api-token",
      null,
      cancellationToken
    );
    response.EnsureSuccessStatusCode();

    var evnt = await response.Content.ReadFromJsonAsync<Event>(cancellationToken);
    if (evnt is null || evnt.Type != "io.eventsourcingdb.api.api-token-verified")
    {
      throw new HttpRequestException("Failed to verify api token");
    }
  }

  public async Task<IEnumerable<Event>> StoreEvents(
    IEnumerable<EventCandidate> eventCandidates,
    IEnumerable<Precondition>? preconditions = null,
    CancellationToken cancellationToken = default
  )
  {
    var candidates = eventCandidates.Select(
      candidate => new Candidate(
        Source: "https://example.com",
        Subject: candidate.Subject,
        Type: candidate.Data.GetType().GetEventType(),
        Data: candidate.Data
      )
    );

    var content = new
    {
      Events = candidates,
      Preconditions = preconditions ?? []
    };
    var httpContent = JsonContent.Create(
      content,
      new MediaTypeHeaderValue("application/json"),
      JsonSerialization.Options
    );

    var response = await _client.PostAsync(
      "write-events",
      httpContent,
      cancellationToken
    );
    await EnsureSuccess("Failed to write events", response, cancellationToken);

    var evnts = await response.Content.ReadFromJsonAsync<IEnumerable<EventResponse>>(cancellationToken);
    if (evnts is null)
    {
      throw new HttpRequestException("Failed to write events");
    }

    return evnts.Select(e => e.Payload);
  }

  private static async Task EnsureSuccess(
    string context,
    HttpResponseMessage response,
    CancellationToken cancellationToken
  )
  {
    if (!response.IsSuccessStatusCode)
    {
      throw new Exception(
        $"{context} {response.StatusCode}: {await response.Content.ReadAsStringAsync(cancellationToken)}"
      );
    }
  }

  public async Task<IAsyncEnumerable<Event>> GetEvents(
    string subject,
    ReadEventsOptions? options = null,
    CancellationToken cancellationToken = default
  )
  {
    var content = new
    {
      Subject = subject,
      Options = options ?? new ReadEventsOptions()
    };
    var httpContent = JsonContent.Create(
      content,
      new MediaTypeHeaderValue("application/json"),
      options: JsonSerialization.Options
    );
    var request = new HttpRequestMessage(HttpMethod.Post, "read-events")
    {
      Content = httpContent
    };

    var response = await _client.SendAsync(
      request,
      HttpCompletionOption.ResponseHeadersRead,
      cancellationToken
    );
    response.EnsureSuccessStatusCode();

    var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

    var events = JsonSerializer.DeserializeAsyncEnumerable<EventResponse>(
      stream,
      true,
      JsonSerialization.Options,
      cancellationToken
    );

    return events.OfType<EventResponse>().Select(e => e.Payload);
  }

  public async Task<IAsyncEnumerable<Event>> RunEventQlQuery(
    string query,
    CancellationToken cancellationToken = default
  )
  {
    var httpContent = JsonContent.Create(
      new { Query = query },
      new MediaTypeHeaderValue("application/json")
    );
    var request = new HttpRequestMessage(HttpMethod.Post, "run-eventql-query")
    {
      Content = httpContent
    };

    var response = await _client.SendAsync(
      request,
      HttpCompletionOption.ResponseHeadersRead,
      cancellationToken
    );
    response.EnsureSuccessStatusCode();

    var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

    var events = JsonSerializer.DeserializeAsyncEnumerable<Response>(
      stream,
      true,
      JsonSerialization.Options,
      cancellationToken
    );

    return events.OfType<EventResponse>().Select(e => e.Payload);
  }
}
