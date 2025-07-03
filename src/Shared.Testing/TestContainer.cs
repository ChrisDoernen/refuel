using DotNet.Testcontainers.Containers;

namespace Shared.Testing;

public abstract class TestContainer : IAsyncDisposable
{
  protected IContainer _container = null!;
  protected readonly bool _randomizeHostPort;
  protected int _port;

  protected TestContainer()
  {
    var randomizeHostPortEnv = Environment.GetEnvironmentVariable("RANDOMIZE_HOST_PORT");
    _randomizeHostPort = randomizeHostPortEnv is not string || bool.Parse(randomizeHostPortEnv);
  }

  public async Task Start(CancellationToken cancellationToken = default)
    => await _container.StartAsync(cancellationToken).ConfigureAwait(false);

  public async ValueTask DisposeAsync() => await _container.DisposeAsync();
}
