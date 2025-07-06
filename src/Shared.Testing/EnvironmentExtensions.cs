namespace Shared.Testing;

public static class EnvironmentUtils
{
  public static bool RandomizeHostPort()
  {
    var randomizeHostPortEnv = Environment.GetEnvironmentVariable("RANDOMIZE_HOST_PORT");

    return randomizeHostPortEnv is not string || bool.Parse(randomizeHostPortEnv);
  }
}
