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
  }

  public Task StopAsync(CancellationToken _) => Task.CompletedTask;
}
