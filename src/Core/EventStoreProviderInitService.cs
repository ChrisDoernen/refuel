using Core.Infrastructure.Cqrs;
using Core.Clubs;
using Core.Clubs.Queries;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Core;

public class EventStoreProviderInitService(
  ILogger<EventStoreProviderInitService> logger,
  IServiceProvider serviceProvider,
  IEventStoreProvider eventStoreProvider
) : IHostedService
{
  public async Task StartAsync(CancellationToken cancellationToken)
  {
    logger.LogDebug("Initializing event store provider");

    using var scope = serviceProvider.CreateScope();
    var mediator = scope.ServiceProvider.GetService<IMediator>()!;

    var clubs = await mediator.Send(new GetClubsQuery(), cancellationToken);
    foreach (var club in clubs)
    {
      eventStoreProvider.RegisterTenant(club.Id, club.TenantId);
    }
  }

  public Task StopAsync(CancellationToken _) => Task.CompletedTask;
}
