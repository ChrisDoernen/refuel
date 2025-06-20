using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace EventSourcingDB;

public class TypeResolver : DefaultJsonTypeInfoResolver
{
  private static readonly Type EventDataType = typeof(IEventData);

  private readonly IEnumerable<Type> _eventDataTypes = AppDomain.CurrentDomain
    .GetAssemblies()
    .SelectMany(a => a.GetTypes())
    .Where(t => EventDataType.IsAssignableFrom(t) && t != EventDataType && t is { IsClass: true, IsAbstract: false });

  public override JsonTypeInfo GetTypeInfo(
    Type type,
    JsonSerializerOptions options
  )
  {
    JsonTypeInfo jsonTypeInfo = base.GetTypeInfo(type, options);

    if (jsonTypeInfo.Type == EventDataType)
    {
      var polymorphismOptions = new JsonPolymorphismOptions
      {
        TypeDiscriminatorPropertyName = "_t",
        IgnoreUnrecognizedTypeDiscriminators = true,
        UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
      };

      foreach (var eventDataType in _eventDataTypes)
      {
        polymorphismOptions.DerivedTypes.Add(new JsonDerivedType(eventDataType, eventDataType.Name));
      }

      jsonTypeInfo.PolymorphismOptions = polymorphismOptions;
    }

    return jsonTypeInfo;
  }
}
