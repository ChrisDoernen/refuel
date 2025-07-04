﻿using Core.Clubs;
using Core.Shared.Authorization;
using EventSourcingDB;
using MediatR;

namespace Core.Tanks.Registration;

public record RegisterTankCommand(
  string Name,
  Guid ClubId,
  string Description,
  int Capacity,
  int FuelLevel = 0
) : IRequest<Guid>;

public class RegisterTankCommandHandler(
  IEventStore eventStore
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
      Subject: $"/tanks/{evnt.TankId}",
      Data: evnt
    );

    await eventStore.StoreEvents(
      [candidate],
      [new IsSubjectPristine(candidate.Subject)],
      cancellationToken
    );

    return evnt.TankId;
  }
}

public class RegisterTankCommandAuthorizer : Authorizer<RegisterTankCommand>
{
  public override async Task BuildPolicy(RegisterTankCommand command)
  {
    UsePolicy(new UserIsInClubRolePolicy(command.ClubId, ClubRoles.Admin));
  }
}
