using Core.Users.SignUp;
using EventSourcingDB;
using MediatR;

namespace Core.Users;

public record GetUsersQuery : IRequest<IEnumerable<User>>;

public class GetUsersQueryHandler(
  IEventStore eventStore,
  IMediator mediator
) : IRequestHandler<GetUsersQuery, IEnumerable<User>>
{
  public async Task<IEnumerable<User>> Handle(
    GetUsersQuery query,
    CancellationToken cancellationToken
  )
  {
    var eventQlQuery =
      $"""
       FROM e IN events
       WHERE e.type == '{EventType.Of<UserSignedUpEventV1>()}'
       PROJECT INTO e
       """;

    var events = await eventStore.RunEventQlQuery(eventQlQuery, cancellationToken);

    return await events
      .Select(e => e.Data)
      .Cast<UserSignedUpEventV1>()
      .SelectAwait(async e => await mediator.Send(new GetUserQuery(e.UserId), cancellationToken))
      .ToListAsync(cancellationToken);
  }
}
