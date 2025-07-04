﻿using EventSourcingDB;

namespace Core.Tanks.Registration;

[EventType("com.example.tank-registered.v1")]
public record TankRegisteredEventV1(
  Guid TankId,
  Guid Club,
  string Name,
  string Description,
  int Capacity,
  int InitialFuelLevel
) :  IEventData;
