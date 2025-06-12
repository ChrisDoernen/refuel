using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Tanks.FuelExtraction;

public record LogFuelExtractedCommand(
  int AmountExtracted
) : IRequest;

public class LogFuelExtractedCommandHandler(
  ILogger<LogFuelExtractedCommandHandler> logger
) : IRequestHandler<LogFuelExtractedCommand>
{
  public Task Handle(
    LogFuelExtractedCommand command,
    CancellationToken cancellationToken
  )
  {
    logger.LogInformation("Fuel extracted command");

    return Task.CompletedTask;
  }
}
