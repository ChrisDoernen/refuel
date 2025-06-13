using System.Text.Json;
using System.Text.Json.Serialization;

namespace EventSourcingDbClient;

public static class JsonSerialization
{
  public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
  {
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    Converters =
    {
      new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
      new PreconditionConverter()
    },
    DefaultBufferSize = 64,
    TypeInfoResolver = new EventDataTypeResolver()
  };
}
