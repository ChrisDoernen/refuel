using Api.Shared;
using Core.Clubs;
using Core.Tanks;
using MediatR;

namespace Api.Tanks;

public class TankType : ObjectType<Tank>
{
  protected override void Configure(IObjectTypeDescriptor<Tank> descriptor)
  {
    descriptor.BindFieldsExplicitly();

    descriptor.Field(t => t.Name);
    descriptor.Field(t => t.Description);
    descriptor.Field(t => t.Capacity);
    descriptor.Field(t => t.RefillRequested);
    descriptor.Field(t => t.FuelLevel);
    descriptor.Field(t => t.AuditTrail);

    descriptor
      .Field("Club")
      .Resolve<Club>(
        async (context, cancellationToken) =>
        {
          var id = context.Parent<Club>().Id;
          var query = new GetClubQuery(id);

          return await context.Service<IMediator>().Send(query, cancellationToken);
        }
      );

    descriptor
      .ImplementsNode()
      .ResolveNode<ClubCompoundId>(
        async (context, id) =>
          await context.Service<IMediator>().Send(new GetTankQuery(id.ClubId, id.Id), context.RequestAborted)
      );
  }
}

[ExtendObjectType(typeof(Tank))]
public class TankExtensions
{
  [ID<Tank>]
  public ClubCompoundId GetId([Parent] Tank tank) => new(tank.ClubId, tank.Id);
}

