using Core.Shared;
using MediatR;

namespace Api.Shared;

public class RoleType : ObjectType<Role>
{
  protected override void Configure(IObjectTypeDescriptor<Role> descriptor)
  {
    descriptor.BindFieldsExplicitly();

    descriptor.Field(r => r.Id).ID();

    descriptor.Field("idValue")
      .Resolve(context => context.Parent<Role>().Id)
      .Type<NonNullType<StringType>>();
    descriptor.Field(r => r.Name);
    descriptor.Field(r => r.Group);
    descriptor.Field(r => r.Description);

    descriptor
      .ImplementsNode()
      .ResolveNode<string>(
        async (context, id) =>
          await context.Service<IMediator>().Send(new GetRoleQuery(id), context.RequestAborted)
      );
  }
}
