using System;
using Arch.Core;
using SFML.Graphics;
using SonicRemake.Layout;

namespace SonicRemake.Systems.Rendering.Debugging
{
  public class LogDebugSystem : GameSystem
  {

    public override void OnRender(World world, RenderWindow window, GameContext context)
    {
      var wrapper = new Layout.Div()
                .Padding(20, 40)
                .Background(new Color(0, 0, 0, 150))
                .Flow(Flow.Vertical)
                .Gap(10);

      for (var i = 0; i < Log.Values.Count; i++)
      {
        var key = Log.Values.Keys.ElementAt(i);
        var value = Log.Values.Values.ElementAt(i);
        var content = $"{key}: {value}";

        var text = new Layout.Text()
          .Content(content)
          .Foreground(Color.White);

        wrapper.Children(text);
      }

      UI.Open(wrapper);
      UI.Close();
    }
  }
}
