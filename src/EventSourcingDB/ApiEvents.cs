namespace EventSourcingDB;

[EventType("io.eventsourcingdb.api.ping-received")]
public record PingReceivedEvent(
  string Message
) : IEventData;

[EventType("io.eventsourcingdb.api.api-token-verified")]
public record ApiTokenVerifiedEvent : IEventData;
