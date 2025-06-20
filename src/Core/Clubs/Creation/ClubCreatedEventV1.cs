using EventSourcingDB;

namespace Core.Clubs.Creation;

[EventType("com.example.club-created.v1")]
public record ClubCreatedEventV1(
  Guid Id,
  string Name,
  string? Description
) :  IEventData;
