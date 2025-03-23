using System;
using Arch.Core;
using SFML.Graphics;

namespace SonicRemake.Systems.Rendering.Debugging
{
  public class LogDebugSystem : GameSystem
  {
    private Font _monocraft = new("Assets/Fonts/Monocraft.ttf");

    public LogDebugSystem()
    {
      // Disable anti-aliasing for Monocraft
      _monocraft.SetSmooth(false);
    }

    public override void OnRender(World world, RenderWindow window, GameContext context)
    {
      for (var i = 0; i < Log.Values.Count; i++)
      {
        var key = Log.Values.Keys.ElementAt(i);
        var value = Log.Values.Values.ElementAt(i);

        var text = new Text($"{key}: {value}", _monocraft, 16)
        {
          Position = new(0, 50 + i * 25),
          FillColor = Color.White,
          CharacterSize = 20
        };

        window.Draw(text);
      }
    }
  }
}
