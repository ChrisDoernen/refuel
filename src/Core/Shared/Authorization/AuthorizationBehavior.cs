using System.Collections.Concurrent;
using System.Reflection;
using MediatR;

namespace Core.Shared.Authorization;

public class AuthorizationBehavior<TRequest, TResponse>(
  IEnumerable<IAuthorizer<TRequest>> authorizers,
  IServiceProvider serviceProvider
) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
  private readonly ConcurrentDictionary<Type, Type> _requirementHandlers = new();
  private readonly ConcurrentDictionary<Type, MethodInfo> _handlerMethodInfo = new();

  public async Task<TResponse> Handle(
    TRequest request,
    RequestHandlerDelegate<TResponse> next,
    CancellationToken cancellationToken
  )
  {
    var policies = new HashSet<IAuthorizationPolicy>();

    foreach (var authorizer in authorizers)
    {
      if (authorizer.Policies.Any())
      {
        authorizer.ClearPolicies();
      }

      await authorizer.BuildPolicy(request);
      foreach (var policy in authorizer.Policies)
      {
        policies.Add(policy);
      }
    }

    if (!policies.Any())
    {
      return await next();
    }

    AuthorizationResult? lastFailure = null;
    foreach (var policy in policies)
    {
      var result = await ExecuteAuthorizationHandler(policy, cancellationToken);
      if (result.IsAuthorized)
      {
        return await next();
      }

      lastFailure = result;
    }

    throw new UnauthorizedAccessException($"Authorization policy not met: {lastFailure!.FailureMessage}");
  }

  private Task<AuthorizationResult> ExecuteAuthorizationHandler(
    IAuthorizationPolicy policy,
    CancellationToken cancellationToken
  )
  {
    var policyType = policy.GetType();
    var handlerType =
      FindHandlerType(policy)
      ?? throw new InvalidOperationException(
        $"Could not find an authorization handler type for policy type {policyType.Name}"
      );
    var handlers = (
      serviceProvider.GetService(typeof(IEnumerable<>).MakeGenericType(handlerType))
        as IEnumerable<object>
    )!.ToList();

    if (handlers == null || !handlers.Any())
    {
      throw new InvalidOperationException(
        $"Could not find an authorization handler implementation for policy type {policyType.Name}"
      );
    }

    if (handlers.Count > 1)
    {
      throw new InvalidOperationException(
        $"Multiple authorization handler implementations were found for policy type \"{policyType.Name}\""
      );
    }

    var serviceHandler = handlers.First();
    var serviceHandlerType = serviceHandler.GetType();

    var methodInfo = _handlerMethodInfo.GetOrAdd(
      serviceHandlerType,
      _ =>
        serviceHandlerType
          .GetMethods()
          .First(x => x.Name == nameof(IAuthorizationHandler<IAuthorizationPolicy>.Handle))
    );

    return (Task<AuthorizationResult>)methodInfo.Invoke(serviceHandler, [policy, cancellationToken])!;
  }

  private Type FindHandlerType(IAuthorizationPolicy policy)
    => _requirementHandlers.GetOrAdd(
      policy.GetType(),
      requirementTypeKey => typeof(IAuthorizationHandler<>).MakeGenericType(requirementTypeKey)
    );
}
