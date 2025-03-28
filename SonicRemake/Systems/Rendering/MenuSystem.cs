using Arch.Core;
using SFML.Graphics;
using SonicRemake.Layout;

namespace SonicRemake.Systems.Rendering;

public class MenuSystem : GameSystem
{
    public override void OnRender(World world, RenderWindow window, GameContext context)
    {
        var background = new Div()
            .Background(Color.Red)
            .Size(Size.Grow)
            .Position(Position.Absolute);

        UI.Open(background);
        UI.Close();
    }
}
