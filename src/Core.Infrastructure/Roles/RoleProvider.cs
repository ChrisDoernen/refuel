﻿using System.Reflection;

namespace Core.Infrastructure.Roles;

public interface IRoleProvider
{
  public IEnumerable<Role> Roles { get; }
  public Role GetRole(string id);
}

public class RoleProvider : IRoleProvider
{
  public IEnumerable<Role> Roles { get; }

  public Role GetRole(string id) =>
    Roles.FirstOrDefault(role => role.Id == id)
    ?? throw new KeyNotFoundException($"Role with id '{id}' was not found.");

  public RoleProvider(Assembly assembly)
  {
    Roles = assembly
      .GetTypes()
      .Where(t => typeof(IRoleDefinition).IsAssignableFrom(t) && t.IsClass)
      .SelectMany(
        type => type.GetFields(BindingFlags.Public | BindingFlags.Static)
          .Where(p => p.FieldType == typeof(Role))
          .Select(p => p.GetValue(null) as Role)
      )
      .OfType<Role>();
  }
}
