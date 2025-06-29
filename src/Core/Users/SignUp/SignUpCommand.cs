﻿using EventSourcingDB;
using MediatR;

namespace Core.Users.SignUp;

public record SignUpCommand(
  string Email,
  string FirstName,
  string LastName
) : IRequest<Guid>;

public class SignUpCommandHandler(
  IEventStore eventStore
) : IRequestHandler<SignUpCommand, Guid>
{
  public async Task<Guid> Handle(
    SignUpCommand command,
    CancellationToken cancellationToken
  )
  {
    var userSignedUpEvent = new UserSignedUpEventV1(
      UserId: Guid.CreateVersion7(),
      FirstName: command.FirstName,
      LastName: command.LastName,
      Email: command.Email
    );
    var candidate = new EventCandidate(
      Subject: $"/users/{userSignedUpEvent.UserId}",
      Data: userSignedUpEvent
    );
    await eventStore.StoreEvents(
      [candidate],
      [new IsSubjectPristine(candidate.Subject)],
      cancellationToken
    );

    return userSignedUpEvent.UserId;
  }
}
