using Core.Users;
using MediatR;

namespace Api.Users;

public class UserType : ObjectType<User>
{
  protected override void Configure(IObjectTypeDescriptor<User> descriptor)
  {
    descriptor.BindFieldsExplicitly();

    descriptor.Field(u => u.Id).ID();
    descriptor.Field(u => u.FirstName);
    descriptor.Field(u => u.LastName);
    descriptor.Field(u => u.ClubRoles);

    descriptor
      .ImplementsNode()
      .ResolveNode<Guid>(
        async (context, id) =>
          await context.Service<IMediator>().Send(new GetUserQuery(id), context.RequestAborted)
      );
  }
}
