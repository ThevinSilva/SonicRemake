using System;
using Arch.Core;
using SonicRemake.Systems;

namespace SonicRemake.Levels;

public class Level(string name)
{
  public string Name { get; private set; } = name;

  public World Entities { get; private set; } = World.Create();

  private readonly List<GameSystem> _systems = [];
  public IReadOnlyList<GameSystem> Systems => _systems.AsReadOnly();

  public void AddSystem(GameSystem system) => _systems.Add(system);

  public void AddSystems(params GameSystem[] systems) => _systems.AddRange(systems);
}
