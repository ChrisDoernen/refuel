using Core.ClubMembership;
using Core.ClubMembership.Commands;
using Core.ClubMembership.Queries;
using Core.Clubs.Commands;
using Core.Tanks.Commands;
using Core.Tanks.Projections;
using Core.Tanks.Queries;
using Core.Users.Commands;
using FluentAssertions;
using MediatR;
using Xunit;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace Core.Tests.Tanks;

public class WorkflowTests(
  ITestOutputHelper testOutputHelper,
  CoreFixture fixture
) : TestBed<CoreFixture>(testOutputHelper, fixture)
{
  private readonly IMediator _mediator = fixture.Get<IMediator>(testOutputHelper);

  [Fact]
  public async Task TankWorkflow()
  {
    var signUpCommand = new SignUpCommand(
      Email: "chris@example.com",
      FirstName: "Chris",
      LastName: "Dörnen"
    );
    var userId = await _mediator.Send(signUpCommand);

    var createClubCommand = new CreateClubCommand(
      Name: "Die Luftakrobaten",
      TenantId: "tenant1",
      Description: "Das ist unser super Verein!"
    );
    var clubId = await _mediator.Send(createClubCommand);

    await _fixture.StartObserver(testOutputHelper, TestContext.Current.CancellationToken);

    var joinClubCommand = new JoinClubCommand(
      UserId: userId,
      ClubId: clubId
    );
    await _mediator.Send(joinClubCommand);

    var assignClubRoleCommand = new AssignClubRoleCommand(
      ClubId: clubId,
      MemberId: userId,
      RoleId: ClubRoles.Admin.Id
    );
    await _mediator.Send(assignClubRoleCommand);

    await _fixture.WaitForProjections();
    var clubMembers = await _mediator.Send(new GetClubMembersQuery(clubId));

    clubMembers.Count().Should().Be(1);
    clubMembers.First().RoleIds.Should().HaveCount(1);

    var registerTankCommand = new RegisterTankCommand(
      Name: "Benzintank H4",
      ClubId: clubId,
      Description: "Großer Benzintank am Hangar 4",
      Capacity: 900,
      FuelLevel: 150
    );
    var tankId = await _mediator.Send(registerTankCommand);

    await _fixture.WaitForProjections();
    
    var getTankQuery = new GetTankQuery(tankId);
    var tank = await _mediator.Send(getTankQuery);

    tank.Should().BeOfType<Tank>();
    tank.FuelLevel.Should().Be(150);
    tank.Meter.Should().BeNull();

    var initializeMeterCommand = new InitializeMeterCommand(
      ClubId: clubId,
      TankId: tankId
    );
    await _mediator.Send(initializeMeterCommand);

    await _fixture.WaitForProjections();
    tank = await _mediator.Send(getTankQuery);

    tank.Meter!.Value.Should().Be(0);

    var logFuelExtractedCommand = new LogFuelExtractedCommand(
      ClubId: clubId,
      TankId: tankId,
      AmountExtracted: 50
    );
    var logMeterReadCommand = new LogMeterReadCommand(
      ClubId: clubId,
      TankId: tankId,
      Value: 50
    );
    await _mediator.Send(logFuelExtractedCommand);
    await _mediator.Send(logMeterReadCommand);

    await _fixture.WaitForProjections();
    tank = await _mediator.Send(getTankQuery);

    tank.FuelLevel.Should().Be(100);
    tank.Meter!.Value.Should().Be(50);

    var logRefillRequested = new LogRefillRequestedCommand(
      ClubId: clubId,
      TankId: tankId
    );
    await _mediator.Send(logRefillRequested);

    await _fixture.WaitForProjections();
    tank = await _mediator.Send(getTankQuery);

    tank.FuelLevel.Should().Be(100);
    tank.RefillRequested.Should().Be(true);

    var logRefilledCommand = new LogRefilledCommand(
      ClubId: clubId,
      TankId: tankId,
      NewFuelLevel: 200
    );
    await _mediator.Send(logRefilledCommand);
    await _fixture.WaitForProjections();
    tank = await _mediator.Send(getTankQuery);

    tank.FuelLevel.Should().Be(200);
    tank.RefillRequested.Should().Be(false);

    var logTooMuchFuelExtractedCommand = new LogFuelExtractedCommand(
      ClubId: clubId,
      TankId: tankId,
      AmountExtracted: 250
    );
    var logTooMuchExtracted = () => _mediator.Send(logTooMuchFuelExtractedCommand);

    await logTooMuchExtracted.Should().ThrowAsync<Exception>();
  }
}
