using System;
using Arch.Core;
using SFML.Graphics;
using SFML.System;
using SonicRemake.Components;
using Transform = SonicRemake.Components.Transform;

namespace SonicRemake.Systems.Rendering.Debugging
{
  public class GridDebugSystem : GameSystem
  {
    private static Log _log = new(typeof(GridDebugSystem));
    private record struct DebugGrid(int X, int Y);

    private static QueryDescription Query = new QueryDescription().WithAll<DebugGrid, Transform>();
    private static QueryDescription CameraQuery = new QueryDescription().WithAll<Components.Camera, Transform>();

    private const int GridSize = 20;

    public override void OnStart(World world)
    {
      for (var x = -GridSize; x <= GridSize; x++)
      {
        for (var y = -GridSize; y <= GridSize; y++)
        {
          world.Create(new DebugGrid(x, y), new Transform(new Vector2f(0, 0), new Vector2f(1, 1)), new Rectangle(new Vector2f(16, 16), new Color(0, 0, 0, 0), new Color(255, 255, 255, 50), 0.5f), new Renderer());
        }
      }
    }

    public override void OnTick(World world, GameContext context)
    {
      var cameraPosition = new Vector2f();
      var cameraZoom = 1f;

      world.Query(in CameraQuery, (Entity entity, ref Components.Camera camera, ref Transform transform) =>
      {
        cameraPosition = transform.Position;
        cameraZoom = camera.Zoom;
      });

      world.Query(in Query, (Entity entity, ref DebugGrid grid, ref Transform transform) =>
      {
        var x = grid.X * 16 + cameraPosition.X;
        var y = grid.Y * 16 + cameraPosition.Y;

        // Align to 16x16 grid
        x = (int)(x / 16) * 16;
        y = (int)(y / 16) * 16;

        transform.Position = new Vector2f(x, y);

      });
    }
  }
}
