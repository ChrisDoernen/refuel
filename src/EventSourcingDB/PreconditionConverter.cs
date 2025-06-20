using System.Text.Json;
using System.Text.Json.Serialization;

namespace EventSourcingDB;

internal class PreconditionConverter : JsonConverter<Precondition>
{
  public override bool CanConvert(Type typeToConvert) => typeof(Precondition).IsAssignableFrom(typeToConvert);

  public override Precondition Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    throw new NotImplementedException();
  }

  public override void Write(
    Utf8JsonWriter writer,
    Precondition precondition,
    JsonSerializerOptions options
  )
  {
    var o = new JsonSerializerOptions(JsonSerializerDefaults.Web);

    if (precondition is IsSubjectPristine isSubjectPristine)
    {
      JsonSerializer.Serialize(writer, isSubjectPristine, o);
    }
    else if (precondition is IsSubjectOnEventId isSubjectOnEventId)
    {
      JsonSerializer.Serialize(writer, isSubjectOnEventId, o);
    }
  }
}
