using EventSourcing;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Core.Infrastructure.Cqrs;

public class EventStoreObserver(
  ILogger<EventStoreObserver> logger,
  IEventStoreProvider eventStoreProvider,
  IServiceProvider serviceProvider
) : BackgroundService
{
  protected override Task ExecuteAsync(CancellationToken cancellationToken)
  {
    logger.LogDebug("Starting event observation");

    foreach (var eventStore in eventStoreProvider.All())
    {
      Task.Run(
        async () =>
        {
          try
          {
            // ToDo: Persist the last processed event id for each event store and start from there when restarting
            var events = eventStore.ObserveEvents(
              new Subject("/"),
              cancellationToken: cancellationToken
            );
            await foreach (var evnt in events)
            {
              using var scope = serviceProvider.CreateScope();
              var mediator = scope.ServiceProvider.GetService<IMediator>()!;

              await mediator.Publish(evnt, cancellationToken);
            }
          }
          catch (Exception ex)
          {
            logger.LogError("Error while processing events: {Message}", ex.Message);

            throw;
          }
        },
        cancellationToken
      );
    }

    return Task.CompletedTask;
  }
}
