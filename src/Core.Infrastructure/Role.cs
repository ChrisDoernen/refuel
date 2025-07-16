namespace Core.Infrastructure;

public record Role(
  string Id,
  string Name,
  string Group,
  string Description
);

/// <summary>
///   Marker interface for module role definitions
/// </summary>
public interface IRoleDefinition;
