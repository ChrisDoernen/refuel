using Core.Users.Commands;

namespace Api.Users;

public class SignUpCommandInputType : InputObjectType<SignUpCommand>
{
  protected override void Configure(IInputObjectTypeDescriptor<SignUpCommand> descriptor)
  {
    descriptor.BindFieldsExplicitly();

    descriptor
      .Field(b => b.FirstName)
      .Description("The first name of the user signing up.");

    descriptor
      .Field(b => b.LastName)
      .Description("The last name of the user signing up.");

    descriptor
      .Field(b => b.Email)
      .Description("The email address of the user signing up.");
  }
}
