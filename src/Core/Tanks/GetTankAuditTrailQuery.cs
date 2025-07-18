using Core.Infrastructure.Cqrs;
using EventSourcing;
using EventSourcingDb.Types;
using MediatR;

namespace Core.Tanks;

public record GetTankAuditTrailQuery(
  Guid ClubId,
  Guid TankId
) : IRequest<AuditTrail<Tank>>;

public class GetTankAuditTrailQueryHandler(
  IAuditTrailReplayService<Tank> replayService
) : IRequestHandler<GetTankAuditTrailQuery, AuditTrail<Tank>>
{
  public async Task<AuditTrail<Tank>> Handle(
    GetTankAuditTrailQuery query,
    CancellationToken cancellationToken
  ) => await replayService.GetAuditTrail(
    query.ClubId,
    new Subject($"/tanks/{query.TankId}"),
    cancellationToken
  );
}
