using Core.Clubs;
using Core.Clubs.Creation;
using Core.Users.AssignClubRole;
using Core.Users.SignUp;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dev;

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
      Description: "Das ist unser super Verein!"
    );
    var clubId = await mediator.Send(createClubCommand, cancellationToken);

    var assignClubRoleCommand = new AssignClubRoleCommand(
      userId,
      new ClubRole(
        clubId,
        ClubRoles.Admin.Id
      )
    );
    await mediator.Send(assignClubRoleCommand, cancellationToken);
  }

  public Task StopAsync(CancellationToken _) => Task.CompletedTask;
}
