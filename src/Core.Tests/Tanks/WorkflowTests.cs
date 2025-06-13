using Core.Tanks;
using Core.Tanks.Querying;
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
    var command = new RegisterTankCommand(
      Name: "Benzintank H4",
      ClubId: "Motorflug Club Phantasien",
      Description: "Großer Benzintank am Hangar 4",
      InitialFuelLevel: 150
    );
    var tankId = await _mediator.Send(command);
    
    var query = new GetTankQuery(tankId);
    var tank = await _mediator.Send(query);

    tank.Should().BeOfType<Tank>();
    tank.FuelLevel.Should().Be(150);
  }
}
