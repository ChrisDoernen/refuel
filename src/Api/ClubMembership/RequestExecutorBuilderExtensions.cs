using HotChocolate.Execution.Configuration;

namespace Api.ClubMembership;

public static class RequestExecutorBuilderExtensions
{
  public static IRequestExecutorBuilder AddClubMembership(this IRequestExecutorBuilder builder)
  {
    return builder
      .AddType<TankRoleAssignmentsType>()
      .AddType<ClubMemberType>();
  }
}
