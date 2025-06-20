using System.Reflection;

namespace EventSourcingDB;

public static class TypeExtensions
{
  public static string GetEventType(this Type eventData)
    => eventData.GetCustomAttribute<EventTypeAttribute>()?.Value ??
       throw new Exception("Event type attribute not found");
}
