using Xunit;

namespace Shared.Testing;

public class NoopTestOutputHelper : ITestOutputHelper
{
  public void Write(string message)
  {
  }

  public void Write(string format, params object[] args)
  {
  }

  public void WriteLine(string message)
  {
  }

  public void WriteLine(string format, params object[] args)
  {
  }

  public string Output { get; }
}
