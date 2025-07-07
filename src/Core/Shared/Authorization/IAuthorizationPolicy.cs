namespace Core.Shared.Authorization;

public interface IAuthorizationPolicy;

public interface IAuthorizationHandler<in TPolicy>
  where TPolicy : IAuthorizationPolicy
{
  Task<AuthorizationResult> Handle(
    TPolicy policy,
    CancellationToken cancellationToken = default
  );
}

public interface IAuthorizer<in T>
{
  IEnumerable<IAuthorizationPolicy> Policies { get; }
  void ClearPolicies();
  Task BuildPolicy(T request);
}

public abstract class Authorizer<T> : IAuthorizer<T>
{
  private readonly HashSet<IAuthorizationPolicy> _policies = [];

  public IEnumerable<IAuthorizationPolicy> Policies => _policies;

  /// <summary>
  ///   Any of the used policies must be fulfilled
  /// </summary>
  protected void UsePolicy(IAuthorizationPolicy policy)
  {
    _policies.Add(policy);
  }

  public virtual void ClearPolicies()
  {
    _policies.Clear();
  }

  public abstract Task BuildPolicy(T request);
}
