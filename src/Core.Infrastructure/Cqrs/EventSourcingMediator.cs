﻿using Core.Infrastructure.Projections;
using EventSourcing;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Infrastructure.Cqrs;

public class EventSourcingMediator(
  IServiceProvider serviceProvider,
  ILogger<EventSourcingMediator> logger,
  IEnumerable<IProjector> projectors
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

    try
    {
      foreach (var projector in projectors)
      {
        await projector.Project(evnt, cancellationToken);
      }
    }
    catch (Exception ex)
    {
      logger.LogError("Error while projecting: {Message}", ex.Message);
      // ToDo ??
    }

    try
    {
      foreach (var handler in handlers)
      {
        var handlerFullName = handler.HandlerInstance.GetType().FullName;

        logger.LogDebug($"Handling event with {handlerFullName}");

        await handler.HandlerCallback(notification, cancellationToken);
      }
    }
    catch (Exception ex)
    {
      logger.LogError("Error while publishing events: {Message}", ex.Message);
      // ToDo ??
    }
  }
}
