
using Arch.Core;
using SFML.System;
using SonicRemake.Components;
using SonicRemake.Systems;
using SonicRemake.Inputs;

namespace SonicRemake.Movement
{
    // IMPLEMENT CONTROL LOCK https://info.sonicretro.org/SPG:Running [bottom]

    public class Movement : GameSystem
    {
        private static Log _log = new(typeof(Movement));

        private QueryDescription Query = new QueryDescription().WithAll<Sonic, Transform, Velocity>();

        // Horizontal Movement Constants https://info.sonicretro.org/SPG:Running
        private const float ACCELERATION_SPEED = 0.046875f;
        private const float DECELERATION_SPEED = 0.5f;
        private const float FRICTION_SPEED = ACCELERATION_SPEED;
        private const float TOP_SPEED = 6.5f;


        // Vertical Movement Constants https://info.sonicretro.org/SPG:Air_State 
        private const float AIR_ACCELERATION_SPEED = ACCELERATION_SPEED * 2;
        private const float GRAVITY = 0.21875f;
        private const float JUMP_FORCE = 6.5f;
        public bool ControlLock { set; get; }

        public void HandleMovement(HashSet<Direction> inputs, ref Transform transform, ref Velocity velocity)
        {
            HandleHorizontalMovement(inputs.Contains(Direction.Backward), inputs.Contains(Direction.Forward), ref velocity);

            HandleVerticalMovement(
                inputs.Contains(Direction.Space),
                inputs.Contains(Direction.Backward),
                inputs.Contains(Direction.Forward),
                ref transform, ref velocity
            );

            transform.Position = new Vector2f(transform.Position.X + velocity.Speed.X, transform.Position.Y + velocity.Speed.Y);
            if (transform.Position.Y >= 100)
            {
                transform.Position = new Vector2f(transform.Position.X, 100);
                transform.IsOnGround = true;
            }
            else
            {
                transform.IsOnGround = false;
            }
        }

        public void HandleHorizontalMovement(bool backward, bool forward, ref Velocity velocity)
        {

            if (forward)
            {
                if (velocity.GroundSpeed < 0) // going backwards
                {
                    velocity.GroundSpeed += DECELERATION_SPEED;

                    if (velocity.GroundSpeed >= 0)
                        velocity.GroundSpeed = DECELERATION_SPEED;
                }
                else if (velocity.GroundSpeed < TOP_SPEED)
                {
                    velocity.GroundSpeed += ACCELERATION_SPEED;

                    if (velocity.GroundSpeed >= TOP_SPEED)
                        velocity.GroundSpeed = TOP_SPEED; //impose top speed limit
                }
            }

            if (backward)
            {
                if (velocity.GroundSpeed > 0) // going forwards
                {
                    velocity.GroundSpeed -= DECELERATION_SPEED;

                    if (velocity.GroundSpeed <= 0)
                        velocity.GroundSpeed = -DECELERATION_SPEED;
                }
                else if (velocity.GroundSpeed > -TOP_SPEED)
                {
                    velocity.GroundSpeed -= ACCELERATION_SPEED;

                    //impose top speed limit
                    if (velocity.GroundSpeed <= -TOP_SPEED)
                        velocity.GroundSpeed = -TOP_SPEED;
                }
            }
            // no horizontal movement
            if (!backward && !forward)
            {
                velocity.GroundSpeed -= Math.Min(Math.Abs(velocity.GroundSpeed), FRICTION_SPEED) * Math.Sign(velocity.GroundSpeed);
            }

        }

        public void HandleVerticalMovement(bool space, bool backward, bool forward, ref Transform transform, ref Velocity velocity)
        {
            var vX = velocity.Speed.X;
            var vY = velocity.Speed.Y;

            if (transform.IsOnGround)
            {
                vX = velocity.GroundSpeed * (float)Math.Cos(transform.GroundAngle);
                vY = velocity.GroundSpeed * -(float)Math.Sin(transform.GroundAngle);
            }
            else
            {
                // Air Drag Added
                if (vY < 0 && vY > -4)
                    vX -= vX / 0.125f / 256;

                // Gravity Force Added
                vY = vY > 16 ? 16 : vY + GRAVITY;

                if (forward)
                    vX += AIR_ACCELERATION_SPEED;

                if (backward)
                    vX -= AIR_ACCELERATION_SPEED;
            }

            if (space && transform.IsOnGround)
            {
                // _log.Debug("bruhhh");
                vX -= JUMP_FORCE * MathF.Sin(transform.GroundAngle);
                vY -= JUMP_FORCE * MathF.Cos(transform.GroundAngle);
            }

            velocity.Speed = new Vector2f(vX, vY);
        }

        public override void OnPhysics(World world, GameContext context)
        {
            world.Query(in Query, (Entity e, ref Sonic s, ref Transform t, ref Velocity v) =>
            {
                HandleMovement(InputSystem.HandleInput(), ref t, ref v);
            });
        }


    }
}
