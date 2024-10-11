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

    private QueryDescription Query = new QueryDescription().WithAll<Sonic, AnimationQueue, Velocity, Transform, Renderer>();

    public override void OnAnimation(World world, RenderWindow window, GameContext context)
    {
        world.Query(in Query, (Entity entity, ref Sonic sonic, ref AnimationQueue queue, ref Velocity velocity, ref Transform transform, ref Renderer renderer) =>
        {
            // Flip the sprite based on the direction
            if (velocity.GroundSpeed < 0)
                renderer.FlipX = true;
            else if (velocity.GroundSpeed > 0)
                renderer.FlipX = false;

            if (transform.IsOnGround)
            {
                var duration = (int)Math.Floor(Math.Max(0, 8 - Math.Abs(velocity.GroundSpeed)));

                queue.FramesPerSprite = duration;

                var inputs = InputSystem.HandleInput();
                var isBreakingForwards = inputs.Contains(Direction.Backward) && velocity.GroundSpeed > 0;
                var isBreakingBackwards = inputs.Contains(Direction.Forward) && velocity.GroundSpeed < 0;

                if (isBreakingForwards || isBreakingBackwards)
                {
                    queue.Animation = "skid";
                    queue.FramesPerSprite = 6;
                }
                else
                {
                    queue.Animation = Math.Abs(velocity.GroundSpeed) switch
                    {
                        0 => "idle",
                        < 3 => "jog",
                        < 6 => "walk",
                        < 10 => "run",
                        < 15 => "dash",
                        _ => "peelout",
                    };
                }
            }
            else
            {
                var duration = (int)Math.Floor(Math.Max(0, 6 - Math.Abs(velocity.Speed.X)));
                queue.Animation = "jump";
                queue.FramesPerSprite = duration;
            }
        });
    }
}
