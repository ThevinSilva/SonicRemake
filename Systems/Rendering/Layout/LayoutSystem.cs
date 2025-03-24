using Arch.Core;
using SFML.Graphics;
using SonicRemake.Layout;
using SonicRemake.Systems;

public class LayoutSystem : GameSystem
{
  public override void OnRender(World world, RenderWindow window, GameContext context)
  {
    // Fit sizing pass
    foreach (Div div in Layout.BreadthFirst())
    {
     
    }
  }

  public override void OnTick(World world, GameContext context)
  {
    Layout.Open(new Div()
      .Size(Sizing.Fit, Sizing.Fit)
      .Background(Color.Blue)
      .Padding(32)
      .Gap(32)
    );

    Layout.Open(new Div()
      .Size(300, 300)
      .Background(Color.Cyan)
    );
    Layout.Close();

    Layout.Open(new Div()
       .Size(350, 200)
      .Background(Color.Yellow)
    );
    Layout.Close();

    Layout.Close();
  }
}
