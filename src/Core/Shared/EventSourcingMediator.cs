using EventSourcing;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Shared;

public class EventSourcingMediator(
  IServiceProvider serviceProvider,
  ILogger<EventSourcingMediator> logger
) : Mediator(serviceProvider)
{
  protected override async Task PublishCore(
    IEnumerable<NotificationHandlerExecutor> handlers,
    INotification notification,
    CancellationToken cancellationToken
  )
  {
    var evnt = (Event)notification;
    logger.LogInformation($"Publishing {EventType.Of(evnt.Data)}");

    foreach (var handler in handlers)
    {
      var handlerFullName = handler.HandlerInstance.GetType().FullName;

      logger.LogDebug($"Handling event with {handlerFullName}");

      await handler.HandlerCallback(notification, cancellationToken);
    }
  }
}
