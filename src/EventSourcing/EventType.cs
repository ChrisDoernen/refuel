using System.Reflection;

namespace EventSourcing;

public static class EventType
{
  public static string Of<T>()
    => typeof(T).GetCustomAttribute<EventTypeAttribute>()?.Value ??
       throw new Exception("Event type attribute not found");

  public static string Of(Type evnt)
    => evnt.GetCustomAttribute<EventTypeAttribute>()?.Value ??
       throw new Exception("Event type attribute not found");

  public static string Of(object evnt) => Of(evnt.GetType());
}
