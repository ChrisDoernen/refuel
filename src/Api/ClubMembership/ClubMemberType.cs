using Api.Shared;

using Core.ClubMembership;
using Core.Clubs;
using Core.Infrastructure;
using Core.Shared;
using Core.Tanks;
using Core.Users;
using MediatR;

namespace Api.ClubMembership;

public class ClubMemberType : ObjectType<ClubMember>
{
  protected override void Configure(IObjectTypeDescriptor<ClubMember> descriptor)
  {
    descriptor.BindFieldsExplicitly();

    descriptor
      .ImplementsNode()
      .ResolveNode<ClubCompoundId>(async (context, id) =>
        {
          var query = new GetClubMemberAuditTrailQuery(id.ClubId, id.Id);
          var clubMember = await context.Service<IMediator>().Send(query, context.RequestAborted);

          return clubMember.CurrentState;
        }
      );

    descriptor.Field(c => c.FirstName);
    descriptor.Field(c => c.LastName);

    descriptor.Field("tanks")
      .Resolve<IEnumerable<TankRoleAssignments>>(async (context, cancellationToken) =>
        {
          var member = context.Parent<ClubMember>();
          var assignments = new List<TankRoleAssignments>();

          var roleQuery = new GetRolesQuery();
          var roles = (await context.Service<IMediator>().Send(roleQuery, cancellationToken)).ToList();

          foreach (var assignment in member.TankRoleAssignments)
          {
            var query = new GetTankAuditTrailQuery(member.ClubId, assignment.Key);

            var tank = await context.Service<IMediator>().Send(query, cancellationToken);

            var assignedRoles = roles.Where(r => assignment.Value.Contains(r.Id));

            var tankRoleAssignments = new TankRoleAssignments
            {
              Tank = tank.CurrentState,
              Roles = assignedRoles
            };
            assignments.Add(tankRoleAssignments);
          }

          return assignments;
        }
      );

    descriptor
      .Field("user")
      .Resolve<User>(async (context, cancellationToken) =>
        {
          var id = context.Parent<ClubMember>().Id;
          var query = new GetUserQuery(id);

          return await context.Service<IMediator>().Send(query, cancellationToken);
        }
      );

    descriptor
      .Field("roles")
      .Resolve<IEnumerable<Role>>(async (context, cancellationToken) =>
        {
          var ids = context.Parent<ClubMember>().RoleIds;
          var query = new GetRolesQuery();

          var roles = await context.Service<IMediator>().Send(query, cancellationToken);

          return roles.Where(r => ids.Contains(r.Id));
        }
      );

    descriptor
      .Field("club")
      .Resolve<Club>(async (context, cancellationToken) =>
        {
          var clubId = context.Parent<ClubMember>().ClubId;
          var query = new GetClubQuery(clubId);

          return await context.Service<IMediator>().Send(query, cancellationToken);
        }
      );
  }
}

[ExtendObjectType(typeof(ClubMember))]
public class ClubMemberExtensions
{
  [ID<ClubMember>]
  public ClubCompoundId GetId([Parent] ClubMember member) => new(member.ClubId, member.Id);
}
