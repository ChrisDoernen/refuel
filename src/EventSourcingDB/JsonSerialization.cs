using System.Text.Json;
using System.Text.Json.Serialization;

namespace EventSourcingDB;

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
    TypeInfoResolver = new TypeResolver()
  };
}
