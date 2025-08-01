using HotChocolate.Execution.Configuration;

namespace Api.Users;

public static class RequestExecutorBuilderExtensions
{
  public static IRequestExecutorBuilder AddUsers(this IRequestExecutorBuilder builder)
  {
    return builder
      .AddTypeExtension<UsersQueryType>()
      .AddTypeExtension<UsersMutationType>()
      .AddType<SignUpCommandInputType>()
      .AddType<UserType>();
  }
}
