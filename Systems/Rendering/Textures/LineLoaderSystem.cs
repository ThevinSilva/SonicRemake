using Arch.Core;
using SFML.Graphics;
using SonicRemake.Components;
using Transform = SonicRemake.Components.Transform;

namespace SonicRemake.Systems.Rendering.Textures
{
  public class LineLoaderSystem : GameSystem
  {
    private static QueryDescription Query = new QueryDescription().WithAll<Renderer, Line, Transform>();

    public override void OnRender(World world, RenderWindow window, GameContext context)
    {
      world.Query(in Query, (Entity entity, ref Renderer renderer, ref Line line, ref Transform transform) =>
      {
        transform.Position = new((line.Start.X + line.End.X) / 2, (line.Start.Y + line.End.Y) / 2);
        transform.Rotation = MathF.Atan2(line.End.Y - line.Start.Y, line.End.X - line.Start.X) * 180 / MathF.PI;
        var lineLength = MathF.Sqrt(MathF.Pow(line.End.X - line.Start.X, 2) + MathF.Pow(line.End.Y - line.Start.Y, 2));

        renderer.Drawable = new SFML.Graphics.RectangleShape()
        {
          Size = new((float)lineLength, line.Thickness),
          FillColor = line.FillColor,
        };
      });
    }
  }
}
