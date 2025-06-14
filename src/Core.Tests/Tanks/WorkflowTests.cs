using Core.Shared;
using Core.Tanks;
using Core.Tanks.FuelExtraction;
using Core.Tanks.Querying;
using Core.Tanks.Refilling;
using Core.Tanks.Registration;
using FluentAssertions;
using MediatR;
using Xunit;
using Xunit.Abstractions;
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
    var registerTankCommand = new RegisterTankCommand(
      Name: "Benzintank H4",
      ClubId: Guid.CreateVersion7(),
      Description: "Großer Benzintank am Hangar 4",
      Capacity: 900,
      FuelLevel: 150
    );
    var tankId = await _mediator.Send(registerTankCommand);

    var getTankQuery = new GetTankQuery(tankId);
    var tank = await _mediator.Send(getTankQuery);

    tank.Should().BeOfType<AuditTrail<Tank>>();
    tank.Count.Should().Be(1);
    tank.CurrentState.FuelLevel.Should().Be(150);

    var logFuelExtractedCommand = new LogFuelExtractedCommand(
      TankId: tankId,
      AmountExtracted: 50
    );
    await _mediator.Send(logFuelExtractedCommand);

    tank = await _mediator.Send(getTankQuery);

    tank.CurrentState.FuelLevel.Should().Be(100);

    var refillCommand = new LogRefilledCommand(
      TankId: tankId,
      NewFuelLevel: 200
    );
    await _mediator.Send(refillCommand);

    tank = await _mediator.Send(getTankQuery);

    tank.Count.Should().Be(3);
    tank.CurrentState.FuelLevel.Should().Be(200);

    var logTooMuchFuelExtractedCommand = new LogFuelExtractedCommand(
      TankId: tankId,
      AmountExtracted: 250
    );
    var logTooMuchExtracted = () => _mediator.Send(logTooMuchFuelExtractedCommand);

    await logTooMuchExtracted.Should().ThrowAsync<Exception>();
  }
}
