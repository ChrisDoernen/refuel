using Core.ClubMembership;
using Core.ClubMembership.AssigningClubRoles;
using Core.ClubMembership.Joining;
using Core.Clubs.Creation;
using Core.Tanks;
using Core.Tanks.FuelExtraction;
using Core.Tanks.MeterInitialization;
using Core.Tanks.MeterReading;
using Core.Tanks.Refilling;
using Core.Tanks.Registration;
using Core.Tanks.RequestRefilling;
using Core.Users.SignUp;
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

    var registerTankCommand = new RegisterTankCommand(
      Name: "Benzintank H4",
      ClubId: clubId,
      Description: "Großer Benzintank am Hangar 4",
      Capacity: 900,
      FuelLevel: 150
    );
    var tankId = await _mediator.Send(registerTankCommand);

    var getTankAuditTrailQuery = new GetTankAuditTrailQuery(clubId, tankId);
    var tank = await _mediator.Send(getTankAuditTrailQuery);

    tank.Should().BeOfType<Tank>();
    tank.Count.Should().Be(1);
    tank.CurrentState.FuelLevel.Should().Be(150);
    tank.CurrentState.Meter.Should().BeNull();

    var initializeMeter = new InitializeMeterCommand(
      ClubId: clubId,
      TankId: tankId
    );
    await _mediator.Send(initializeMeter);

    tank = await _mediator.Send(getTankAuditTrailQuery);

    tank.CurrentState.Meter!.Value.Should().Be(0);

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

    tank = await _mediator.Send(getTankAuditTrailQuery);

    tank.CurrentState.FuelLevel.Should().Be(100);
    tank.CurrentState.Meter!.Value.Should().Be(50);

    var logRefillRequested = new LogRefillRequestedCommand(
      ClubId: clubId,
      TankId: tankId
    );
    await _mediator.Send(logRefillRequested);

    tank = await _mediator.Send(getTankAuditTrailQuery);

    tank.CurrentState.FuelLevel.Should().Be(100);
    tank.CurrentState.RefillRequested.Should().Be(true);

    var logRefilledCommand = new LogRefilledCommand(
      ClubId: clubId,
      TankId: tankId,
      NewFuelLevel: 200
    );
    await _mediator.Send(logRefilledCommand);

    tank = await _mediator.Send(getTankAuditTrailQuery);

    tank.Count.Should().Be(6);
    tank.CurrentState.FuelLevel.Should().Be(200);
    tank.CurrentState.RefillRequested.Should().Be(false);

    var logTooMuchFuelExtractedCommand = new LogFuelExtractedCommand(
      ClubId: clubId,
      TankId: tankId,
      AmountExtracted: 250
    );
    var logTooMuchExtracted = () => _mediator.Send(logTooMuchFuelExtractedCommand);

    await logTooMuchExtracted.Should().ThrowAsync<Exception>();
  }
}
