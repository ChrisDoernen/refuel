using System.Globalization;
using System.Text.Json;

namespace EventSourcing;

public class EventConverter
{
  private static readonly Type EventDataType = typeof(IEventData);
  private readonly Dictionary<string, Type> _eventDataTypes;

  private static readonly JsonSerializerOptions DefaultSerializerOptions = new()
  {
    PropertyNameCaseInsensitive = true
  };

  public EventConverter()
  {
    _eventDataTypes = AppDomain.CurrentDomain
      .GetAssemblies()
      .SelectMany(a => a.GetTypes())
      .Where(t => EventDataType.IsAssignableFrom(t) && t != EventDataType && t is { IsClass: true, IsAbstract: false })
      .ToDictionary(EventType.Of, t => t);
  }

  public Event Convert(EventSourcingDb.Types.Event evnt)
  {
    var type =
      _eventDataTypes.GetValueOrDefault(evnt.Type)
      ?? throw new Exception($"Event data type '{evnt.Type}' not found");

    var eventData =
      evnt.Data.Deserialize(type, DefaultSerializerOptions) as IEventData ??
      throw new Exception($"Failed to deserialize event data of type '{evnt.Type}'");

    // EventSourcingDb guarantees Id is a valid uint
    var id = System.Convert.ToUInt32(evnt.Id, CultureInfo.InvariantCulture);

    var subject = Subject.Parse(evnt.Subject);

    return new Event
    {
      Data = eventData,
      Id = id,
      Time = evnt.Time,
      Source = evnt.Source,
      Subject = subject
    };
  }
}
