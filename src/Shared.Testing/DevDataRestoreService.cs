using Core.ClubMembership;
using Core.ClubMembership.AssignClubRole;
using Core.ClubMembership.JoiningClubs;
using Core.Clubs.Creation;
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
    await mediator.Send(registerTankCommand, cancellationToken);
  }

  public Task StopAsync(CancellationToken _) => Task.CompletedTask;
}
