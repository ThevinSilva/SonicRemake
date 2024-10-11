using System;
using System.ComponentModel;
using Arch.Core;
using SFML.Graphics;
using SFML.System;
using SonicRemake.Components;
using Transform = SonicRemake.Components.Transform;

namespace SonicRemake.Systems.Rendering.Camera;

public class CameraSystem : GameSystem
{
  private QueryDescription CameraQuery = new QueryDescription().WithAll<Components.Camera, Transform>();
  private QueryDescription SonicQuery = new QueryDescription().WithAll<Sonic, Transform>();

  public override void OnPhysics(World world, GameContext context)
  {
    var sonicPosition = new Vector2f();

    world.Query(in SonicQuery, (Entity entity, ref Sonic sonic, ref Transform transform) =>
    {
      sonicPosition = transform.Position;
    });

    world.Query(in CameraQuery, (Entity entity, ref Components.Camera camera, ref Transform transform) =>
    {
      // Lerp the camera to the Sonic's position
      var lerp = 0.1f;
      var cameraPosition = transform.Position;
      var targetPosition = new Vector2f(sonicPosition.X, sonicPosition.Y);
      var newPosition = new Vector2f(
        cameraPosition.X + (targetPosition.X - cameraPosition.X) * lerp,
        cameraPosition.Y + (targetPosition.Y - cameraPosition.Y) * lerp
      );

      transform.Position = newPosition;
    });
  }
}
