using System.Diagnostics;
using Core.Infrastructure.Utils;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Infrastructure.Cqrs;

public class LoggingBehavior<TRequest, TResponse>(
  ILogger<LoggingBehavior<TRequest, TResponse>> logger
) : IPipelineBehavior<TRequest, TResponse> where TRequest : IBaseRequest
{
  public async Task<TResponse> Handle(
    TRequest request,
    RequestHandlerDelegate<TResponse> next,
    CancellationToken cancellationToken
  )
  {
    var requestName = typeof(TRequest).Name;
    logger.LogDebug($"Start handling {requestName}");

    var (result, timeElapsedInMs) = await TimeTracker.Execute(async () => await next());

    logger.LogDebug($"Finished handling {requestName} in {timeElapsedInMs} ms");

    return result;
  }
}
