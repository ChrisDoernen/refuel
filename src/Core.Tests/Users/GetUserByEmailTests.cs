using Core.Users;
using Core.Users.SignUp;
using FluentAssertions;
using MediatR;
using Xunit;

using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace Core.Tests.Users;

public class GetUserByEmailTests(
  ITestOutputHelper testOutputHelper,
  CoreFixture fixture
) : TestBed<CoreFixture>(testOutputHelper, fixture)
{
  private readonly IMediator _mediator = fixture.Get<IMediator>(testOutputHelper);

  [Fact]
  public async Task GetUserByEmail()
  {
    var signedUpEvent = new SignUpCommand(
      Email: "chris.doernen@example.com",
      FirstName: "Chris",
      LastName: "Dörnen"
    );
    var userId = await _mediator.Send(signedUpEvent);

    var user = await _mediator.Send(new GetUserByEmailQuery(signedUpEvent.Email));

    user.Id.Should().Be(userId);
    user.Email.Should().Be(signedUpEvent.Email);
    user.FirstName.Should().Be(signedUpEvent.FirstName);
    user.LastName.Should().Be(signedUpEvent.LastName);
  }
}
