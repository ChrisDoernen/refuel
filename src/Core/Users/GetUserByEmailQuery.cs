using Core.Users.SignUp;
using EventSourcingDB;
using MediatR;

namespace Core.Users;

public record GetUserByEmailQuery(
  string Email
) : IRequest<User>;

public class GetUserByEmailQueryHandler(
  IEventStore eventStore,
  IMediator mediator
) : IRequestHandler<GetUserByEmailQuery, User>
{
  public async Task<User> Handle(
    GetUserByEmailQuery query,
    CancellationToken cancellationToken
  )
  {
    var eventQlQuery =
      $"""
       FROM e IN events
       WHERE e.type == '{EventType.Of<UserSignedUpEventV1>()}' AND e.data.email == '{query.Email}'
       TOP 1
       PROJECT INTO e
       """;

    var events = await eventStore.RunEventQlQuery(eventQlQuery, cancellationToken);

    var userSignedUpEvent = await events
      .Select(e => e.Data)
      .Cast<UserSignedUpEventV1>()
      .FirstOrDefaultAsync(cancellationToken);

    if (userSignedUpEvent is null)
    {
      throw new Exception($"User with email {query.Email} did not yet sign up");
    }

    var user = await mediator.Send(
      new GetUserQuery(userSignedUpEvent.UserId),
      cancellationToken
    );

    return user;
  }
}
