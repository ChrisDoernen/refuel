using HotChocolate.Execution.Configuration;

namespace Api.Shared;

public static class RequestExecutorBuilderExtensions
{
  public static IRequestExecutorBuilder AddShared(this IRequestExecutorBuilder builder)
  {
    return builder
      .AddTypeExtension<SharedQueryType>()
      .AddType<RoleType>()
      .AddType<EventType>();
  }
}
