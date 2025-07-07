using Core.Users;
using MediatR;

namespace Api.Users;

public class UserType : ObjectType<User>
{
  protected override void Configure(IObjectTypeDescriptor<User> descriptor)
  {
    descriptor.BindFieldsExplicitly();

    descriptor
      .ImplementsNode()
      .IdField(u => u.Id)
      .ResolveNode(
        async (context, id) =>
          await context.Service<IMediator>().Send(new GetUserQuery(id), context.RequestAborted)
      );

    descriptor.Field(u => u.FirstName);
    descriptor.Field(u => u.LastName);
  }
}
