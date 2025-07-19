using Core.Infrastructure.Authorization;
using Core.Infrastructure.Cqrs;
using Core.Tanks.Events;
using EventSourcing;
using MediatR;

namespace Core.Tanks.Commands;

public record RegisterTankCommand(
  string Name,
  Guid ClubId,
  string Description,
  int Capacity,
  int FuelLevel = 0
) : IRequest<Guid>;

public class RegisterTankCommandHandler(
  IEventStoreProvider eventStoreProvider
) : IRequestHandler<RegisterTankCommand, Guid>
{
  public async Task<Guid> Handle(
    RegisterTankCommand command,
    CancellationToken cancellationToken
  )
  {
    var evnt = new TankRegisteredEventV1(
      Guid.CreateVersion7(),
      command.ClubId,
      command.Name,
      command.Description,
      command.Capacity,
      command.FuelLevel
    );

    var candidate = new EventCandidate(
      Subject: new Subject($"/tanks/{evnt.TankId}"),
      Data: evnt
    );

    await eventStoreProvider
      .ForClub(command.ClubId)
      .StoreEvents(
        [candidate],
        [new IsSubjectPristinePrecondition(candidate.Subject)],
        cancellationToken
      );

    return evnt.TankId;
  }
}

public class RegisterTankCommandAuthorizer : Authorizer<RegisterTankCommand>
{
  public override async Task BuildPolicy(RegisterTankCommand command)
  {
    // UsePolicy(new ClubMemberHasRolePolicy(command.ClubId, ClubRoles.Admin));
  }
}
