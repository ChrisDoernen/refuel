using EventSourcingDbClient;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Tanks.Registration;

public record RegisterTankCommand(
  string Name,
  Guid ClubId,
  string Description,
  int Capacity,
  int FuelLevel = 0
) : IRequest<Guid>;

public class RegisterTankCommandHandler(
  ILogger<RegisterTankCommandHandler> logger,
  IEventStore eventStore
) : IRequestHandler<RegisterTankCommand, Guid>
{
  public async Task<Guid> Handle(
    RegisterTankCommand command,
    CancellationToken cancellationToken
  )
  {
    logger.LogInformation("Register tank command");

    var evnt = new TankRegisteredEventV1(
      Guid.CreateVersion7(),
      command.ClubId,
      command.Name,
      command.Description,
      command.Capacity,
      command.FuelLevel
    );

    var candidate = new EventCandidate(
      Subject: $"/tanks/{evnt.Id}",
      Data: evnt
    );

    await eventStore.StoreEvents(
      [candidate],
      [new IsSubjectPristine(candidate.Subject)],
      cancellationToken
    );

    return evnt.Id;
  }
}
