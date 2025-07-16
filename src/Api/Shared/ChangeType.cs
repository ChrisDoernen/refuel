using EventSourcing;

namespace Api.Shared;

public class EventType : ObjectType<Event>
{
  protected override void Configure(IObjectTypeDescriptor<Event> descriptor)
  {
    descriptor.BindFieldsExplicitly();
    descriptor.Field(e => e.Id);
    descriptor.Field(e => e.Subject);
    descriptor.Field(e => e.Time);
  }
}
