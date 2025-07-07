using Core.ClubMembership;
using Core.Clubs;
using Core.Tanks;
using MediatR;

namespace Api.Clubs;

public class ClubType : ObjectType<Club>
{
  protected override void Configure(IObjectTypeDescriptor<Club> descriptor)
  {
    descriptor.BindFieldsExplicitly();

    descriptor
      .ImplementsNode()
      .IdField(c => c.Id)
      .ResolveNode(
        async (context, id) =>
          await context.Service<IMediator>().Send(new GetClubQuery(id), context.RequestAborted)
      );

    descriptor.Field(c => c.Name);
    descriptor.Field(c => c.Description);

    descriptor
      .Field("tanks")
      .Resolve<IEnumerable<Tank>>(
        async (context, cancellationToken) =>
        {
          var clubId = context.Parent<Club>().Id;
          var query = new GetTanksOfClubQuery(clubId);

          return await context.Service<IMediator>().Send(query, cancellationToken);
        }
      );

    descriptor
      .Field("members")
      .Resolve<IEnumerable<ClubMember>>(
        async (context, cancellationToken) =>
        {
          var id = context.Parent<Club>().Id;
          var query = new GetClubMembersQuery(id);

          return await context.Service<IMediator>().Send(query, cancellationToken);
        }
      );
  }
}
