using Core.ClubMembership;
using Core.ClubMembership.AssigningClubRoles;
using Core.ClubMembership.AssigningTankRoles;
using Core.ClubMembership.Joining;
using Core.Clubs.Creation;
using Core.Tanks;
using Core.Tanks.Registration;
using Core.Users.SignUp;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Shared.Testing;

public class DevDataRestoreService(
  IServiceProvider services,
  ILogger<DevDataRestoreService> logger
) : IHostedService
{
  public async Task StartAsync(CancellationToken cancellationToken)
  {
    logger.LogInformation("Restoring dev data");

    using var scope = services.CreateScope();
    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

    var signUpCommand = new SignUpCommand(
      Email: "chris@example.com",
      FirstName: "Chris",
      LastName: "Dörnen"
    );
    var userId = await mediator.Send(signUpCommand, cancellationToken);

    var createClubCommand = new CreateClubCommand(
      Name: "Die Luftakrobaten",
      TenantId: "tenant1",
      Description: "Das ist unser super Verein!"
    );
    var clubId = await mediator.Send(createClubCommand, cancellationToken);

    var joinClubCommand = new JoinClubCommand(
      UserId: userId,
      ClubId: clubId
    );
    await mediator.Send(joinClubCommand, cancellationToken);

    var assignClubRoleCommand = new AssignClubRoleCommand(
      clubId,
      userId,
      ClubRoles.Admin.Id
    );
    await mediator.Send(assignClubRoleCommand, cancellationToken);

    var registerTankCommand = new RegisterTankCommand(
      Name: "Benzintank H4",
      ClubId: clubId,
      Description: "Großer Benzintank am Hangar 4",
      Capacity: 900,
      FuelLevel: 150
    );
    var tankId = await mediator.Send(registerTankCommand, cancellationToken);

    var assignTankRoleCommand = new AssignTankRoleCommand(
      ClubId: clubId,
      TankId: tankId,
      MemberId: userId,
      RoleId: TankRoles.Responsible.Id
    );
    await mediator.Send(assignTankRoleCommand, cancellationToken);
  }

  public Task StopAsync(CancellationToken _) => Task.CompletedTask;
}
