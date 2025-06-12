namespace EventSourcingDbClient.Tests;

[EventType("com.example.test-event.v1")]
public record TestEventV1(
  Guid Id
): IEventData;

[EventType("com.example.other-test-event.v1")]
public record OtherTestEventV1(
  Guid Id
): IEventData;
