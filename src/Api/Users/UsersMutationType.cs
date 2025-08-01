using Api.GraphQL;
using Core.Users;
using Core.Users.Commands;
using MediatR;

namespace Api.Users;

public class UsersMutationType : ObjectTypeExtension<Mutation>
{
  protected override void Configure(IObjectTypeDescriptor<Mutation> descriptor)
  {
    descriptor
      .Field("signUp")
      .Argument("command", a => a.Type<SignUpCommandInputType>())
      .Resolve(async (context, cancellationToken) =>
        {
          var command = context.ArgumentValue<SignUpCommand>("command");

          return await context.Service<IMediator>().Send(command, cancellationToken);
        }
      );
  }
}
