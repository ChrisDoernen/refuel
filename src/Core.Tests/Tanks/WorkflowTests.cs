using Core.Shared;
using Core.Tanks;
using Core.Tanks.FuelExtraction;
using Core.Tanks.MeterInitialization;
using Core.Tanks.MeterReading;
using Core.Tanks.Refilling;
using Core.Tanks.Registration;
using Core.Tanks.RequestRefilling;
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
    tank.CurrentState.Meter.Should().BeNull();

    var initializeMeter = new InitializeMeterCommand(
      TankId: tankId
    );
    await _mediator.Send(initializeMeter);
    
    tank = await _mediator.Send(getTankQuery);

    tank.CurrentState.Meter!.Value.Should().Be(0);
    
    var logFuelExtractedCommand = new LogFuelExtractedCommand(
      TankId: tankId,
      AmountExtracted: 50
    );
    var logMeterReadCommand = new LogMeterReadCommand(
      TankId: tankId,
      Value: 50
    );
    await _mediator.Send(logFuelExtractedCommand);
    await _mediator.Send(logMeterReadCommand);
    
    tank = await _mediator.Send(getTankQuery);

    tank.CurrentState.FuelLevel.Should().Be(100);
    tank.CurrentState.Meter!.Value.Should().Be(50);

    var logRefillRequested = new LogRefillRequestedCommand(
      TankId: tankId
    );
    await _mediator.Send(logRefillRequested);

    tank = await _mediator.Send(getTankQuery);

    tank.CurrentState.FuelLevel.Should().Be(100);
    tank.CurrentState.RefillRequested.Should().Be(true);
    
    var logRefilledCommand = new LogRefilledCommand(
      TankId: tankId,
      NewFuelLevel: 200
    );
    await _mediator.Send(logRefilledCommand);

    tank = await _mediator.Send(getTankQuery);

    tank.Count.Should().Be(6);
    tank.CurrentState.FuelLevel.Should().Be(200);
    tank.CurrentState.RefillRequested.Should().Be(false);

    var logTooMuchFuelExtractedCommand = new LogFuelExtractedCommand(
      TankId: tankId,
      AmountExtracted: 250
    );
    var logTooMuchExtracted = () => _mediator.Send(logTooMuchFuelExtractedCommand);

    await logTooMuchExtracted.Should().ThrowAsync<Exception>();
  }
}
