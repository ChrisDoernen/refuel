using System.Text.Json;
using System.Text.Json.Serialization;
using EventSourcing;

namespace Core.Infrastructure.Serialization;

public class SubjectConverter : JsonConverter<Subject>
{
  public override Subject Read(
    ref Utf8JsonReader reader,
    Type typeToConvert,
    JsonSerializerOptions options
  ) => Subject.Parse(reader.GetString()!);

  public override void Write(
    Utf8JsonWriter writer,
    Subject subject,
    JsonSerializerOptions options
  ) => writer.WriteStringValue(subject.ToString());
}
