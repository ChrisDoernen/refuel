using DotNet.Testcontainers.Containers;

namespace Shared.Testing;

public abstract class TestContainer : IAsyncDisposable
{
  protected IContainer _container = null!;
  protected readonly bool _randomizeHostPort = EnvironmentUtils.RandomizeHostPort();
  protected int _port;

  public async Task Start(CancellationToken cancellationToken = default)
    => await _container.StartAsync(cancellationToken);

  public async ValueTask DisposeAsync() => await _container.DisposeAsync();
}

public static class TestContainerExtensions
{
  public static async Task Start(
    this IEnumerable<TestContainer> containers
  )
  {
    foreach (var container in containers)
    {
      await container.Start();
    }
  }

  public static async Task Dispose(
    this IEnumerable<TestContainer> containers
  )
  {
    foreach (var container in containers)
    {
      await container.DisposeAsync();
    }
  }
}
