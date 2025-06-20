using Core.Shared;
using EventSourcingDB;
using MediatR;

namespace Core.Users;

public record GetUserByEmailQuery(
  string Email
) : IRequest<User>;

public class GetUserByEmailQueryHandler(
  IEventStore eventStore
) : IRequestHandler<GetUserByEmailQuery, User>
{
  public async Task<User> Handle(
    GetUserByEmailQuery query,
    CancellationToken cancellationToken
  )
  {
    throw new NotImplementedException();
  }
}
