using Core.Clubs.Creation;
using Core.Shared;
using EventSourcingDB;

namespace Core.Clubs;

public record Club : IReplayable<Club>
{
  public Guid Id { get; private set; }
  public string Name { get; private set; } = null!;
  public string? Description { get; private set; }

  public Club Apply(IEventData evnt)
  {
    return evnt switch
    {
      ClubCreatedEventV1 clubCreatedEvent => Apply(clubCreatedEvent),
      _ => throw new InvalidOperationException("Unknown event for club"),
    };
  }

  private Club Apply(ClubCreatedEventV1 evnt) => this with
  {
    Id = evnt.Id,
    Name = evnt.Name,
    Description = evnt.Description
  };
}
