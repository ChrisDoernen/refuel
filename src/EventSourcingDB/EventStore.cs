using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace EventSourcingDB;

public class EventStore(
  IHttpClientFactory factory,
  IOptions<EventSourcingDbOptions> options
) : IEventStore, IEventSourcingDbClient
{
  private readonly HttpClient _client = factory.CreateClient("eventsourcingdb");

  public async Task Ping(CancellationToken cancellationToken)
  {
    var response = await _client.GetAsync("ping", cancellationToken);
    response.EnsureSuccessStatusCode();

    var evnt = await response.Content.ReadFromJsonAsync<UtilityEndpointEvent>(cancellationToken);
    if (evnt is null || evnt.Type != EventType.Of<PingReceivedEvent>())
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

    var evnt = await response.Content.ReadFromJsonAsync<UtilityEndpointEvent>(cancellationToken);
    if (evnt is null || evnt.Type != EventType.Of<ApiTokenVerifiedEvent>())
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
        Source: options.Value.Source,
        Subject: candidate.Subject,
        Type: EventType.Of(candidate.Data),
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
      options: JsonSerialization.Options
    );

    var response = await _client.PostAsync(
      "write-events",
      httpContent,
      cancellationToken
    );
    await EnsureSuccess("Failed to write events", response, cancellationToken);

    var responses = await response.Content.ReadFromJsonAsync<IEnumerable<EventResponse>>(cancellationToken);
    if (responses is null)
    {
      throw new HttpRequestException("Failed to write events");
    }

    return responses.Select(e => e.Payload);
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

    var responses = JsonSerializer.DeserializeAsyncEnumerable<EventResponse>(
      stream,
      true,
      JsonSerialization.Options,
      cancellationToken
    );

    return responses.OfType<EventResponse>().Select(e => e.Payload);
  }

  public async Task<IAsyncEnumerable<EventProjection>> RunEventQlQuery(
    string query,
    CancellationToken cancellationToken = default
  )
  {
    var httpContent = JsonContent.Create(new { Query = query });
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

    var responses = JsonSerializer.DeserializeAsyncEnumerable<Response>(
      stream,
      true,
      JsonSerialization.Options,
      cancellationToken
    );

    return responses.OfType<ProjectionResponse>().Select(e => e.Payload);
  }
}
