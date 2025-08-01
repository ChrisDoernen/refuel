using HotChocolate.Execution.Configuration;

namespace Api.Tanks;

public static class RequestExecutorBuilderExtensions
{
  public static IRequestExecutorBuilder AddTanks(this IRequestExecutorBuilder builder)
  {
    return builder
      .AddType<TankType>();
  }
}
