﻿using Core.Infrastructure.Cqrs;
using Core.Clubs;
using Core.Clubs.Queries;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Core;

public class EventStoreProviderInitService(
  ILogger<EventStoreProviderInitService> logger,
  IEventStoreProvider eventStoreProvider,
  IMediator mediator
) : IHostedService
{
  public async Task StartAsync(CancellationToken cancellationToken)
  {
    logger.LogInformation("Initializing event store provider");

    var clubs = await mediator.Send(new GetClubsQuery(), cancellationToken);
    foreach (var club in clubs)
    {
      eventStoreProvider.RegisterTenant(club.Id, club.TenantId);
    }
  }

  public Task StopAsync(CancellationToken _) => Task.CompletedTask;
}
