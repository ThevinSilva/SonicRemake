using Arch.Core;
using SFML.Graphics;
using SonicRemake.Inputs;
using SonicRemake.Layout;
using static SFML.Window.Keyboard;

namespace SonicRemake.Systems.Rendering;

public class MenuSystem : GameSystem
{
    private bool _isMenuOpen = false;

    public override void OnTick(World world, GameContext context)
    {
        if (Input.IsKeyEnded(Key.Escape))
            _isMenuOpen = !_isMenuOpen;
    }

    public override void OnRender(World world, RenderWindow window, GameContext context)
    {
        if (!window.HasFocus())
            _isMenuOpen = true;

        if (!_isMenuOpen)
            return;

        var wrapper = new Div("menu wrapper")
            .Size(Size.Grow)
            .Position(Position.Absolute)
            .Padding(20)
            .Align(Align.Center);

        var background = new Div("menu background")
            .Size(400, 200)
            .Padding(20)
            .Border(new Color(255, 255, 255, 100), 3)
            .Background(new Color(0, 0, 0, 240));

        var title = new Layout.Text("menu title")
            .Content("GAME PAUSED")
            .Size(Size.Grow)
            .Border(Color.White, 2)
            .Foreground(Color.Blue);

        wrapper.Children(background.Children(title));

        UI.Open(wrapper);
        UI.Close();
    }
}
