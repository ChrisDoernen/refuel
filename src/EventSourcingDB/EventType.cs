using System.Reflection;

namespace EventSourcingDB;

public static class EventType
{
  public static string Of<T>()
    => typeof(T).GetCustomAttribute<EventTypeAttribute>()?.Value ??
       throw new Exception("Event type attribute not found");

  public static string Of(object evnt)
    => evnt.GetType().GetCustomAttribute<EventTypeAttribute>()?.Value ??
       throw new Exception("Event type attribute not found");
}
