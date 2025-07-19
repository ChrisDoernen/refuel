using Core.Infrastructure.Cqrs;
using Core.Tanks.Events;
using Core.Tanks.Projections;
using EventSourcing;
using MediatR;

namespace Core.Tanks.Commands;

public record LogMeterReadCommand(
  Guid ClubId,
  Guid TankId,
  int Value
) : IRequest;

public class LogMeterReadCommandHandler(
  IReplayService<Tank> replayService,
  IEventStoreProvider eventStoreProvider
) : IRequestHandler<LogMeterReadCommand>
{
  public async Task Handle(
    LogMeterReadCommand command,
    CancellationToken cancellationToken
  )
  {
    var auditTrail = await replayService.GetAuditTrail(
      command.ClubId,
      new Subject($"/tanks/{command.TankId}"),
      cancellationToken
    );
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
