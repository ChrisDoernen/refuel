using Core.Clubs;
using Core.Tanks;
using MediatR;

namespace Api.Clubs;

public class ClubType : ObjectType<Club>
{
  protected override void Configure(IObjectTypeDescriptor<Club> descriptor)
  {
    descriptor.BindFieldsExplicitly();
    descriptor.Field(c => c.Id).ID();
    descriptor.Field(c => c.Name);
    descriptor.Field(c => c.Description);
    descriptor.Field(c => c.AuditTrail);

    descriptor
      .ImplementsNode()
      .ResolveNode<Guid>(
        async (context, id) =>
        {
          var query = new GetClubQuery(id);
          return await context.Service<IMediator>().Send(query, context.RequestAborted);
        }
      );

    descriptor
      .Field("Tanks")
      .Resolve<IEnumerable<Tank>>(
        async (context, cancellationToken) =>
        {
          var id = context.Parent<Club>().Id;
          // await context.Service<IMediator>()

          return new List<Tank>();
        }
      );
  }
}
