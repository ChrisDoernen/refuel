using Core.Clubs;
using Core.Shared;
using MediatR;

namespace Api.Clubs;

public class ClubRoleType : ObjectType<ClubRole>
{
  protected override void Configure(IObjectTypeDescriptor<ClubRole> descriptor)
  {
    descriptor.BindFieldsExplicitly();

    descriptor.Field(c => c.ClubId);

    descriptor
      .Field("role")
      .Resolve<Role>(
        async (context, cancellationToken) =>
        {
          var id = context.Parent<ClubRole>().RoleId;
          var query = new GetRoleQuery(id);

          return await context.Service<IMediator>().Send(query, cancellationToken);
        }
      );
  }
}
