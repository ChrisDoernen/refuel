using Core.Infrastructure;
using Core.Infrastructure.Roles;
using Core.Roles;
using Core.Roles.Queries;
using MediatR;

namespace Api.Shared;

public class RoleType : ObjectType<Role>
{
  protected override void Configure(IObjectTypeDescriptor<Role> descriptor)
  {
    descriptor.BindFieldsExplicitly();

    descriptor
      .ImplementsNode()
      .IdField(r => r.Id)
      .ResolveNode(
        async (context, id) =>
          await context.Service<IMediator>().Send(new GetRoleQuery(id), context.RequestAborted)
      );

    descriptor
      .Field("idValue")
      .Resolve(context => context.Parent<Role>().Id)
      .Type<NonNullType<StringType>>();
    descriptor.Field(r => r.Name);
    descriptor.Field(r => r.Group);
    descriptor.Field(r => r.Description);
  }
}
