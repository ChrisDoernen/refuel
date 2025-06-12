namespace EventSourcingDbClient;

public interface IEventSourcingDbClient
{
  Task Ping(CancellationToken cancellationToken = default);

  Task VerifyApiToken(CancellationToken cancellationToken = default);
}
