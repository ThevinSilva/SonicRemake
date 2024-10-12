using System;
using Arch.Core;

namespace SonicRemake.Levels;

public class Level(string name)
{
  public string Name { get; private set; } = name;

  public World Entities { get; private set; } = World.Create();
}
