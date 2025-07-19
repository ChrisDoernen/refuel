using Core.ClubMembership;
using Core.Roles;
using Core.Roles.Queries;
using FluentAssertions;
using MediatR;
using Xunit;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace Core.Tests.Shared;

public class GetRolesTests(
  ITestOutputHelper testOutputHelper,
  CoreFixture fixture
) : TestBed<CoreFixture>(testOutputHelper, fixture)
{
  private readonly IMediator _mediator = fixture.Get<IMediator>(testOutputHelper);

  [Fact]
  public async Task GetRoles()
  {
    var roles = (await _mediator.Send(new GetRolesQuery())).ToList();

    roles.Should().NotBeEmpty();
    roles.Should().Contain(ClubRoles.Admin);
  }
}
