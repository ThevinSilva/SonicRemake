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
  private QueryDescription SonicQuery = new QueryDescription().WithAll<Sonic, Transform, Velocity>();

  public override void OnPhysics(World world, GameContext context)
  {
    var sonicPosition = new Vector2f();
    var sonicVelocity = 0f;

    world.Query(in SonicQuery, (Entity entity, ref Sonic sonic, ref Transform transform, ref Velocity velocity) =>
    {
      sonicPosition = transform.Position;
      sonicVelocity = velocity.Speed.X;
    });

    world.Query(in CameraQuery, (Entity entity, ref Components.Camera camera, ref Transform transform) =>
    {
      var multiplier = sonicVelocity > 0 ? 1 : -1;
      var cameraXOffset = sonicVelocity * 30;

      var cameraPosition = transform.Position;
      var targetPosition = new Vector2f(sonicPosition.X, sonicPosition.Y);
      var newPosition = new Vector2f(
        Lerp(cameraPosition.X, targetPosition.X + cameraXOffset, 0.05f),
        Lerp(cameraPosition.Y, targetPosition.Y, 0.1f)
      );

      transform.Position = newPosition;
    });
  }
  private static float Lerp(float a, float b, float t)
  {
    t = Math.Clamp(t, 0, 1);
    return a + (b - a) * t;
  }
}
