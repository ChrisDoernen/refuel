using Core.Infrastructure.Cqrs;
using EventSourcing;

namespace Core.Infrastructure.ReadModels;

public record IdentifiedReadModelsState<T>
  : ReadModelState where T : IIdentifiedReadModel, IReplayable<T>, new()
{
  public required Dictionary<Guid, T> ReadModels { get; init; }
}

public record AggregatingReadModelsState<T>
  : ReadModelState where T : IIdentifiedReadModel, IReplayable<T>, new()
{
  public required T ReadModels { get; init; }
}

public record ReadModelState
{
  public required EventMetadata Metadata { get; init; }
}
