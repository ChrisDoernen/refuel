using MediatR;

namespace Core.Tanks.Refilling;

public record LogRefilledCommand(
  int NewFuelLevel
) : ITankRelated, IRequest;
