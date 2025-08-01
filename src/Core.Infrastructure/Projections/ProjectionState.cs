﻿using Core.Infrastructure.Cqrs;
using EventSourcing;

namespace Core.Infrastructure.Projections;

public record IdentifiedProjectionState<T>
  : ProjectionState where T : IIdentifiedProjection, IReplayable<T>, new()
{
  public required Dictionary<Guid, T> Projections { get; init; }
}

public record AggregatingProjectionState<T>
  : ProjectionState where T : IIdentifiedProjection, IReplayable<T>, new()
{
  public required T Projections { get; init; }
}

public record ProjectionState
{
  public required EventMetadata Metadata { get; init; }
}
