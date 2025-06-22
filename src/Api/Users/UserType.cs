using Core.Users;

namespace Api.Users;

public class UserType : ObjectType<User>
{
  protected override void Configure(IObjectTypeDescriptor<User> descriptor)
  {
    descriptor.BindFieldsExplicitly();
    descriptor.Field(c => c.Id).ID();
    descriptor.Field(c => c.FirstName);
    descriptor.Field(c => c.LastName);
  }
}
