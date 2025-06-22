using Core.Shared;

namespace Api.Shared;

public class StateChangeType<TSchemaType, TRuntimeType> : ObjectType<StateChange<TRuntimeType>>
  where TSchemaType : class, IOutputType
{
  protected override void Configure(IObjectTypeDescriptor<StateChange<TRuntimeType>> descriptor)
  {
    descriptor.Field(c => c.Change);
    descriptor
      .Field(c => c.State)
      .Type<NonNullType<TSchemaType>>();
  }
}
