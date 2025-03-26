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
      var grid = new Layout.Div()
                .Background(new Color(0, 0, 0, 150))
                //.Background(Color.Red)
                .Flow(Flow.Horizontal)
                .Padding(20)
                .Gap(25);

      var keys = new Layout.Div()
                .Flow(Flow.Vertical)
                .Size(200, Size.Fit)
                .Gap(25);

      var values = new Layout.Div()
                .Flow(Flow.Vertical)
                .Size(200, Size.Fit)
                .Gap(25);

      grid.Children(keys, values);

      for (var i = 0; i < Log.Values.Count; i++)
      {
        var key = Log.Values.Keys.ElementAt(i);
        var value = Log.Values.Values.ElementAt(i);

        var keyText = new Layout.Text()
          .Content(key)
          .Foreground(Color.White);

        var valueText = new Layout.Text()
          .Content(value.ToString() ?? "")
          .Foreground(Color.White);

        keys.Children(keyText);
        values.Children(valueText);
      }

      UI.Open(grid);
      UI.Close();
    }
  }
}
