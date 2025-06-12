using Xunit.Abstractions;

namespace Shared.Testing;

public class NoopTestOutputHelper : ITestOutputHelper
{
  public void WriteLine(string message)
  {
  }

  public void WriteLine(string format, params object[] args)
  {
  }
}
