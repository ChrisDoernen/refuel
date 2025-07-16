using Core.Infrastructure.Cqrs;

namespace Api.Shared;

public class StateChangeType<TSchemaType, TRuntimeType> : ObjectType<StateChange<TRuntimeType>>
  where TSchemaType : class, IOutputType where TRuntimeType : IReplayable<TRuntimeType>, new()
{
  protected override void Configure(IObjectTypeDescriptor<StateChange<TRuntimeType>> descriptor)
  {
    descriptor.Field(c => c.ProcessedEvent);
    descriptor
      .Field(c => c.State)
      .Type<NonNullType<TSchemaType>>();
  }
}
