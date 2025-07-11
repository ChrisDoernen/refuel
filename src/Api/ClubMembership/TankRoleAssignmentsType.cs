using App;
using Core.Shared;
using Core.Tanks;

namespace Api.ClubMembership;

public class TankRoleAssignmentsType : ObjectType<TankRoleAssignments>
{
  protected override void Configure(IObjectTypeDescriptor<TankRoleAssignments> descriptor)
  {
    descriptor.BindFieldsExplicitly();

    descriptor.Field(a => a.Tank);
    descriptor.Field(a => a.Roles);
  }
}

public class TankRoleAssignments
{
  public required Tank Tank { get; init; }
  public required IEnumerable<Role> Roles { get; init; }
}
