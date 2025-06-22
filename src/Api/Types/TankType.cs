using Core.Clubs;
using Core.Tanks;
using MediatR;

namespace Api.Types;

public class TankType : ObjectType<Tank>
{
  protected override void Configure(IObjectTypeDescriptor<Tank> descriptor)
  {
    descriptor.Field(t => t.Id).ID();

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
  }
}
