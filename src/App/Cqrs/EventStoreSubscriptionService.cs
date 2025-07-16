using EventSourcing;
using EventSourcingDb.Types;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace App.Cqrs;

public class EventStoreSubscriptionService(
  ILogger<EventStoreSubscriptionService> logger,
  IEventStoreProvider eventStoreProvider,
  IMediator mediator
) : IHostedService, IDisposable
{
  private readonly CancellationTokenSource _cancellationTokenSource = new();

  public Task StartAsync(CancellationToken cancellationToken)
  {
    logger.LogInformation("Starting EventStore subscriptions");

    foreach (var eventStore in eventStoreProvider.All())
    {
      Task.Run(
        async () =>
        {
          // ToDo: Persist the last processed event id for each event store and start from there when restarting
          var events = eventStore.GetEvents(
            new Subject("/"),
            new ReadEventsOptions(true),
            _cancellationTokenSource.Token
          );
          await foreach (var evnt in events)
          {
            if (_cancellationTokenSource.IsCancellationRequested)
            {
              break;
            }

            // ToDo: Scope?
            await mediator.Publish(evnt, _cancellationTokenSource.Token);
          }
        },
        cancellationToken
      );
    }

    return Task.CompletedTask;
  }

  public Task StopAsync(CancellationToken cancellationToken)
  {
    _cancellationTokenSource.Cancel();

    return Task.CompletedTask;
  }

  public void Dispose()
  {
    _cancellationTokenSource.Dispose();
  }
}
