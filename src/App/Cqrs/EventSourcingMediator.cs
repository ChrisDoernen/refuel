using EventSourcing;
using MediatR;
using Microsoft.Extensions.Logging;

namespace App.Cqrs;

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
    logger.LogInformation($"Publishing {EventType.Of(((Event)notification).Data)}");

    foreach (var handler in handlers)
    {
      var handlerFullName = handler.HandlerInstance.GetType().FullName;

      logger.LogDebug($"Handling event with {handlerFullName}");

      await handler.HandlerCallback(notification, cancellationToken);
    }
  }
}
