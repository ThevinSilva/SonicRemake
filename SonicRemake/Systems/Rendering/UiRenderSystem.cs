using Arch.Core;
using SFML.Graphics;
using SFML.System;
using SonicRemake.Layout;

namespace SonicRemake.Systems.Rendering;


public class UiRenderSystem : GameSystem
{
    public override void OnRender(World world, RenderWindow window, GameContext context)
    {
        UI.Calculate();

        foreach (var node in UI.BreadthFirst())
        {
            if (node.Parent == null)
                continue;

            var rect = new RectangleShape()
            {
                FillColor = node.Background,
                Size = new Vector2f(node.Width.Calculated, node.Height.Calculated),
                Position = new Vector2f(node.Position.X, node.Position.Y),
                Scale = new Vector2f(1, 1),
            };

            window.Draw(rect);
        }

        UI.Init(window.Size.X, window.Size.Y);

        var square = new Node()
            .Size(Size.Grow, Size.Grow)
            .Padding(20)
            .Gap(20);

        var child1 = new Node()
            .Size(Size.Grow)
            .Background(Color.Blue);

        var child2 = new Node()
            .Size(Size.Grow)
            .Background(Color.Green);

        var child3 = new Node()
            .Size(Size.Grow)
            .Background(Color.Yellow);

        square.Children(child1, child2, child3);

        UI.Open(square);

        UI.Close();
    }

}
