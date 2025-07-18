namespace Core.Infrastructure.ReadModels;

/// <summary>
///   Marks read models that have many uniquely identified instances.
/// </summary>
public interface IIdentifiedReadModel
{
  public Guid Id { get; }
}

/// <summary>
///   Marks read models that aggregate arbitrary event data into a single instance.
/// </summary>
public interface IAggregatingReadModel;
