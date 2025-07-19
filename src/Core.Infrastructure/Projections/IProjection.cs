namespace Core.Infrastructure.Projections;

/// <summary>
///   Marks projections that have many uniquely identified instances.
/// </summary>
public interface IIdentifiedProjection
{
  public Guid Id { get; }
}

/// <summary>
///   Marks projections that aggregate arbitrary event data into a single instance.
/// </summary>
public interface IAggregatingProjection;
