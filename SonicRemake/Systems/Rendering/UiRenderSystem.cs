using Arch.Core;
using SFML.Graphics;
using SFML.System;
using SonicRemake.Layout;

namespace SonicRemake.Systems.Rendering;


public class UiRenderSystem : GameSystem
{
    private readonly Log log = new(typeof(UiRenderSystem));
    private readonly Font _monocraft = new("Assets/Fonts/Monocraft.ttf");

    public UiRenderSystem()
    {
        _monocraft.SetSmooth(false);
    }

    public override void OnRender(World world, RenderWindow window, GameContext context)
    {
        UI.Calculate();

        foreach (var node in UI.BreadthFirst())
        {
            if (!node.WorthRendering || node.Parent == null)
                continue;

            if (node is Layout.Text textNode)
            {
                var textObject = new SFML.Graphics.Text(textNode.Content, _monocraft)
                {
                    FillColor = node.Foreground,
                    Position = new Vector2f(node.Position.Calculated.X, node.Position.Calculated.Y),
                    Scale = new Vector2f(1, 1),
                    OutlineColor = textNode.Border.Color,
                    OutlineThickness = textNode.Border.Thickness,
                };

                textObject.CharacterSize = 20;

                window.Draw(textObject);
            }
            else if (node is Div div)
            {
                var rect = new RectangleShape()
                {
                    FillColor = node.Background,
                    Size = new Vector2f(node.Width.Calculated, node.Height.Calculated),
                    Position = new Vector2f(node.Position.Calculated.X, node.Position.Calculated.Y),
                    Scale = new Vector2f(1, 1),
                    OutlineColor = div.Border.Color,
                    OutlineThickness = div.Border.Thickness,
                };

                window.Draw(rect);
            }
        }

        UI.Init(window.Size.X, window.Size.Y);
    }
}
