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
    Sonic sonic = default;
    Transform sonicTransform = default;
    Velocity sonicVelocity = default;

    world.Query(in SonicQuery, (Entity entity, ref Sonic s, ref Transform t, ref Velocity v) =>
    {
      sonic = s;
      sonicVelocity = v;
      sonicTransform = t;
    });

    world.Query(in CameraQuery, (Entity entity, ref Components.Camera camera, ref Transform transform) =>
    {
      var multiplier = sonic.Facing == Facing.Right ? 1 : -1;

      var spinDashOffset = MathF.Floor(sonic.SpinRef) / 2f * multiplier * 30;
      var velocityOffset = sonicVelocity.GroundSpeed * 15;

      var cameraXOffset = spinDashOffset + velocityOffset;


      var cameraPosition = transform.Position;
      var targetPosition = new Vector2f(sonicTransform.Position.X, sonicTransform.Position.Y);
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
