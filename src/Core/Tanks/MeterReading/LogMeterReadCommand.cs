using EventSourcingDB;
using MediatR;

namespace Core.Tanks.MeterReading;

public record LogMeterReadCommand(
  Guid TankId,
  int Value
) : IRequest;

public class LogMeterReadCommandHandler(
  IMediator mediator,
  IEventStore eventStore
) : IRequestHandler<LogMeterReadCommand>
{
  public async Task Handle(
    LogMeterReadCommand command,
    CancellationToken cancellationToken
  )
  {
    var tank = await mediator.Send(new GetTankQuery(command.TankId), cancellationToken);

    if (command.Value < tank.Meter?.Value)
    {
      throw new Exception("Meter read that is lower than the current value.");
    }

    var meterReadEvent = new MeterReadEventV1(command.Value);
    var candidate = new EventCandidate(
      Subject: $"/tanks/{command.TankId}/meter",
      Data: meterReadEvent
    );
    await eventStore.StoreEvents(
      [candidate],
      cancellationToken: cancellationToken
    );
  }
}
