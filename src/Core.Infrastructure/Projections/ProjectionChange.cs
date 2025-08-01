﻿using Core.Infrastructure.Cqrs;
using EventSourcing;

namespace Core.Infrastructure.Projections;

/// <summary>
///   Contains a state together with the event that led to this state.
///   To be used on the read side, the event does not contain the whole event data.
/// </summary>
public record ProjectionChange<T>(
  EventMetadata EventMetadata,
  T State
) where T : IReplayable<T>, new();

public static class ProjectionChangeExtensions
{
  public static ProjectionChange<T> Apply<T>(
    this ProjectionChange<T> stateChange,
    Event evnt
  ) where T : IReplayable<T>, new()
  {
    if (evnt.Id <= stateChange.EventMetadata.Id)
    {
      return stateChange;
    }

    var newState = stateChange.State.Apply(evnt.Data);

    return new ProjectionChange<T>(evnt, newState);
  }
}
