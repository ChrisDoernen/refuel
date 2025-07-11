namespace App.Authorization;

public class AuthorizationResult
{
  public bool IsAuthorized { get; }
  public string? FailureMessage { get; set; }

  private AuthorizationResult(
    bool isAuthorized,
    string? failureMessage
  )
  {
    IsAuthorized = isAuthorized;
    FailureMessage = failureMessage;
  }

  public static AuthorizationResult Fail(string? failureMessage = null) => new(false, failureMessage);

  public static AuthorizationResult Succeed() => new(true, null);
}
