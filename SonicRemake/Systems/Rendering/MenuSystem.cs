using Arch.Core;
using SFML.Graphics;
using SonicRemake.Common;
using SonicRemake.Inputs;
using SonicRemake.Layout;
using SonicRemake.Layout.Engine;
using static SFML.Window.Keyboard;

namespace SonicRemake.Systems.Rendering;

public class MenuSystem : GameSystem
{
    private bool _isMenuOpen = false;
    private readonly TextureHandle _backgroundTexture = TextureHelper.CreateHandle("sonic_mania_general.png", new Color(38, 123, 218));

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
            .Size(127 * 4, 91 * 4)
            .Background(TextureHelper.CreateHandle(_backgroundTexture, 2, 3366, 127, 91));
        ;
        wrapper.Children(background);

        UI.Open(wrapper);
        UI.Close();
    }
}
