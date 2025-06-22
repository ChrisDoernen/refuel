using Core.Clubs;
using Core.Tanks;
using MediatR;

namespace Api.Types;

public class ClubType : ObjectType<Club>
{
  protected override void Configure(IObjectTypeDescriptor<Club> descriptor)
  {
    descriptor.Field(c => c.Id).ID();
    
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
