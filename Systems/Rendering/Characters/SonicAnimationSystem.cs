using System;
using Arch.Core;
using SFML.Graphics;
using SonicRemake.Components;
using SonicRemake.Inputs;
using Transform = SonicRemake.Components.Transform;

namespace SonicRemake.Systems.Rendering.Characters;

public class SonicAnimationSystem : GameSystem
{
    private static Log _log = new(typeof(SonicAnimationSystem));

    private QueryDescription Query = new QueryDescription().WithAll<Sonic, SpriteAnimation, Velocity, Transform, Renderer>();

    public override void OnAnimation(World world, RenderWindow window, GameContext context)
    {
        world.Query(in Query, (Entity entity, ref Sonic sonic, ref SpriteAnimation queue, ref Velocity velocity, ref Transform transform, ref Renderer renderer) =>
        {
            if (sonic.Facing == Facing.Left)
                renderer.FlipX = true;
            else renderer.FlipX = false;

            if (sonic.IsOnGround)
            {
                var duration = (int)Math.Floor(Math.Max(1, 8 - Math.Abs(velocity.GroundSpeed)));

                queue.FramesPerSprite = duration;

                var inputs = InputSystem.HandleInput();

                switch (sonic.State)
                {
                    case SonicState.Charging:
                        queue.Animation = "spin_dash";
                        queue.FramesPerSprite = (int)Math.Floor(Math.Max(1, 8 - sonic.SpinRef));
                        break;
                    case SonicState.Skidding:
                        queue.Animation = "skid";
                        queue.FramesPerSprite = 6;
                        break;
                    case SonicState.Running:
                        queue.Animation = Math.Abs(velocity.GroundSpeed) switch
                        {
                            0 => "idle",
                            < 3 => "jog",
                            < 6 => "walk",
                            < 10 => "run",
                            < 15 => "dash",
                            _ => "peelout",
                        };
                        break;
                    case SonicState.Idle:
                        queue.Animation = "idle";
                        break;
                }
            }
            else
            {
                var duration = (int)Math.Floor(Math.Max(1, 6 - Math.Abs(velocity.Speed.X)));
                queue.Animation = "jump";
                queue.FramesPerSprite = duration;
            }
        });
    }
}
