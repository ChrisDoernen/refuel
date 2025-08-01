using Api.Clubs;
using HotChocolate.Execution.Configuration;

namespace Api.Clubs;

public static class RequestExecutorBuilderExtensions
{
  public static IRequestExecutorBuilder AddClubs(this IRequestExecutorBuilder builder)
  {
    return builder
      .AddTypeExtension<ClubsQueryType>()
      .AddType<ClubType>();
  }
}
