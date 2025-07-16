using Core.Infrastructure.Cqrs;
using EventSourcing;
using MediatR;

namespace Core.Tanks.MeterReading;

public record LogMeterReadCommand(
  Guid ClubId,
  Guid TankId,
  int Value
) : IRequest;

public class LogMeterReadCommandHandler(
  IMediator mediator,
  IEventStoreProvider eventStoreProvider
) : IRequestHandler<LogMeterReadCommand>
{
  public async Task Handle(
    LogMeterReadCommand command,
    CancellationToken cancellationToken
  )
  {
    var auditTrail = await mediator.Send(new GetTankAuditTrailQuery(command.ClubId, command.TankId), cancellationToken);
    var tank = auditTrail.CurrentState;

    if (command.Value < tank.Meter?.Value)
    {
      throw new Exception("Meter read that is lower than the current value.");
    }

    var meterReadEvent = new MeterReadEventV1(command.Value);
    var candidate = new EventCandidate(
      Subject: new Subject($"/tanks/{command.TankId}/meter"),
      Data: meterReadEvent
    );
    await eventStoreProvider
      .ForClub(command.ClubId)
      .StoreEvents(
      [candidate],
      cancellationToken: cancellationToken
    );
  }
}
