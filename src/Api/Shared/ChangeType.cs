using Core.Shared;

namespace Api.Shared;

public class ChangeType : ObjectType<Change>
{
  protected override void Configure(IObjectTypeDescriptor<Change> descriptor)
  {
    descriptor.BindFieldsExplicitly();
    descriptor.Field(c => c.Subject);
    descriptor.Field(c => c.Time);
  }
}
