using Core.Users.AssignClubRole;
using EventSourcingDB;
using MediatR;

namespace Core.Users;

public record GetUsersInClubQuery(
  Guid ClubId
) : IRequest<IEnumerable<User>>;

public class GetUsersOfClubQueryHandler(
  IEventStore eventStore,
  IMediator mediator
) : IRequestHandler<GetUsersInClubQuery, IEnumerable<User>>
{
  public async Task<IEnumerable<User>> Handle(
    GetUsersInClubQuery query,
    CancellationToken cancellationToken
  )
  {
    var eventQlQuery =
      $"""
       FROM e IN events
       WHERE e.type == '{EventType.Of<ClubRoleAssignedEventV1>()}'
         AND e.data.clubRole.roleId == '{query.ClubId}'
       PROJECT INTO e
       """;

    var events = await eventStore.RunEventQlQuery(eventQlQuery, cancellationToken);

    return await events
      .Select(e => e.Data)
      .Cast<ClubRoleAssignedEventV1>()
      .SelectAwait(async e => await mediator.Send(new GetUserQuery(e.UserId), cancellationToken))
      .ToListAsync(cancellationToken);
  }
}
