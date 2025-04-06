using Arch.Core;
using SonicRemake.Components;
using Transform = SonicRemake.Components.Transform;

namespace SonicRemake.Systems.Rendering.Characters;

public class SonicAnimationSystem : GameSystem
{
    private static Log _log = new(typeof(SonicAnimationSystem));

    private QueryDescription Query = new QueryDescription().WithAll<Sonic, SpriteAnimation, Velocity, Transform, Renderer, Sensors, AnimationSequence>();

    public override void OnTick(World world, GameContext context)
    {
        world.Query(in Query, (Entity entity, ref Sonic sonic, ref SpriteAnimation queue, ref Velocity velocity, ref Transform transform, ref Renderer renderer, ref Components.Sensors sensors, ref AnimationSequence sequence) =>
        {
            if (sonic.Facing == Facing.Left)
                renderer.FlipX = true;
            else renderer.FlipX = false;

            if (sonic.IsOnGround)
            {
                var duration = (int)Math.Floor(Math.Max(0, 8 - Math.Abs(velocity.GroundSpeed)));
                queue.FramesPerSprite = duration;

                switch (sonic.State)
                {
                    case SonicState.Crouching:
                        queue.Animation = "crouch";
                        queue.FramesPerSprite = 6;
                        queue.Loop = false;
                        break;
                    case SonicState.Charging:
                        queue.Animation = "spin_dash";
                        queue.FramesPerSprite = (int)Math.Floor(Math.Max(0, 8 - sonic.SpinRef));
                        queue.Loop = true;
                        break;
                    case SonicState.Skidding:
                        queue.Animation = "skid";
                        queue.FramesPerSprite = 6;
                        queue.Loop = false;
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
                        queue.Loop = true;
                        break;
                    case SonicState.Bored:
                        sequence.Names = ["boredOne_transition", "boredOne_main", "boredTwo_transition", "boredTwo_transition_2", "boredTwo_main", "boredTwo_end"];
                        sequence.Loop = false;
                        break;
                    case SonicState.BalanceBackward:
                        queue.Animation = "BalanceTwo";
                        queue.Loop = true;
                        break;
                    case SonicState.BalanceForward:
                        queue.Animation = "BalanceTwo";
                        queue.Loop = true;
                        break;
                    case SonicState.Push:
                        queue.Animation = "push";
                        queue.Loop = true;
                        break;
                    case SonicState.Idle:
                        queue.Animation = "idle";
                        queue.Loop = false;
                        break;
                }
            }
            else
            {
                var position = sensors.LowerRight.Position;
                var intersection = new[] { sensors.LowerLeft, sensors.LowerRight }.OrderBy(sensor => sensor.Distance).First().Intersection;

                if (!intersection.HasValue || Math.Abs(intersection.Value.Y - position.Y) > 4)
                {
                    var duration = (int)Math.Floor(Math.Max(0, 6 - Math.Abs(velocity.Speed.X)));
                    queue.Animation = "jump";
                    queue.FramesPerSprite = duration;
                    queue.Loop = true;
                }



            }
        });
    }
}
