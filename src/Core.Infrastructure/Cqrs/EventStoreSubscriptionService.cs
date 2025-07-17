using EventSourcing;
using EventSourcingDb.Types;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Core.Infrastructure.Cqrs;

public class EventStoreSubscriptionService(
  ILogger<EventStoreSubscriptionService> logger,
  IEventStoreProvider eventStoreProvider,
  IServiceProvider serviceProvider
) : IHostedService, IDisposable
{
  private readonly CancellationTokenSource _cancellationTokenSource = new();

  public Task StartAsync(CancellationToken cancellationToken)
  {
    logger.LogInformation("Starting EventStore subscriptions");

    Task.Run(
      async () =>
      {
        try
        {
          foreach (var eventStore in eventStoreProvider.All())
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

              using var scope = serviceProvider.CreateScope();
              var mediator = scope.ServiceProvider.GetService<IMediator>()!;

              await mediator.Publish(evnt, _cancellationTokenSource.Token);
            }
          }
        }
        catch (Exception ex)
        {
          logger.LogError("Error while processing events: {Message}", ex.Message);
        }
      },
      cancellationToken
    );

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
