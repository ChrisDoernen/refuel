namespace EventSourcing;

[AttributeUsage(AttributeTargets.Class)]
public sealed class EventTypeAttribute(
  string value
) : Attribute
{
  public string Value { get; } = value;
}
