using Arch.Core;
using SFML.Graphics;
using SonicRemake.Components;

namespace SonicRemake.Systems.Rendering.Textures;

public class RectangleLoaderSystem : GameSystem
{
    private static Log _log = new(typeof(RectangleLoaderSystem));

    private static QueryDescription Query = new QueryDescription().WithAll<Renderer, Rectangle>();

    public override void OnRender(World world, RenderWindow window, GameContext context)
    {
        world.Query(in Query, (Entity entity, ref Renderer renderer, ref Rectangle rectangle) =>
        {
            if (renderer.Drawable == null)
            {
                renderer.Drawable = new RectangleShape()
                {
                    Size = rectangle.Size,
                    FillColor = rectangle.FillColor,
                    OutlineColor = rectangle.OutlineColor,
                    OutlineThickness = rectangle.OutlineThickness
                };
            }
        });
    }
}
