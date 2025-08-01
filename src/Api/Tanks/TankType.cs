using Api.Shared;
using Core.Clubs.Models;
using Core.Clubs.Queries;
using Core.Tanks.Projections;
using Core.Tanks.Queries;
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

    descriptor
      .Field("Club")
      .Resolve<Club>(async (context, cancellationToken) =>
        {
          var id = context.Parent<Club>().Id;
          var query = new GetClubQuery(id);

          return await context.Service<IMediator>().Send(query, cancellationToken);
        }
      );

    descriptor
      .ImplementsNode()
      .ResolveNode<Guid>(async (context, id) =>
        {
          var query = new GetTankQuery(id);

          return await context.Service<IMediator>().Send(query, context.RequestAborted);
        }
      );
  }
}
