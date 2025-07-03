using MongoDB;

namespace Core.Clubs;

public record Club : Document
{
  public string Name { get; private set; } = null!;
  public string? Description { get; private set; }

  public Club(string name, string? description)
  {
    Name = name;
    Description = description;
  }
}
