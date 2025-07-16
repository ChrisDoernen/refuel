using Core.Infrastructure.ReadModels;
using EventSourcing;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Infrastructure.Cqrs;

public class EventSourcingMediator(
  IServiceProvider serviceProvider,
  ILogger<EventSourcingMediator> logger,
  IEnumerable<IReadModelSynchronizationService> readModelSynchronizationServices
) : Mediator(serviceProvider)
{
  protected override async Task PublishCore(
    IEnumerable<NotificationHandlerExecutor> handlers,
    INotification notification,
    CancellationToken cancellationToken
  )
  {
    var evnt = notification as Event ?? throw new Exception("Notification is not an Event");

    logger.LogInformation($"Publishing {EventType.Of(evnt.Data)}");

    foreach (var synchronizationService in readModelSynchronizationServices)
    {
      await synchronizationService.Replay(evnt, cancellationToken);
    }

    foreach (var handler in handlers)
    {
      var handlerFullName = handler.HandlerInstance.GetType().FullName;

      logger.LogDebug($"Handling event with {handlerFullName}");

      await handler.HandlerCallback(notification, cancellationToken);
    }
  }
}
