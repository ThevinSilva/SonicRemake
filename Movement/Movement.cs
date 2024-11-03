
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

        private QueryDescription Query = new QueryDescription().WithAll<Sonic, Transform, Velocity, Sensors>();

        // Horizontal Movement Constants https://info.sonicretro.org/SPG:Running
        private const float ACCELERATION_SPEED = 0.046875f;
        private const float DECELERATION_SPEED = 0.5f;
        private const float FRICTION_SPEED = ACCELERATION_SPEED;
        private const float TOP_SPEED = 6.5f;


        // Vertical Movement Constants https://info.sonicretro.org/SPG:Air_State 
        private const float AIR_ACCELERATION_SPEED = ACCELERATION_SPEED * 2;
        private const float GRAVITY = 0.21875f;
        private const float JUMP_FORCE = 6.5f;

        public override void OnTick(World world, GameContext context)
        {
            world.Query(in Query, (Entity e, ref Sonic s, ref Transform t, ref Velocity v, ref Sonic sonic, ref Sensors sensors) =>
            {
                HandleMovement(ref t, ref v, ref sonic, ref sensors);
            });
        }

        private void HandleMovement(ref Transform transform, ref Velocity velocity, ref Sonic sonic, ref Sensors sensors)
        {
            // if (sensors.LowerLeft.Intersection.HasValue && sensors.LowerRight.Intersection.HasValue && (sonic.State != SonicState.Jumping && sonic.State != SonicState.Falling))
            // {
            //     var angle = MathF.Atan2(sensors.LowerLeft.Intersection.Value.Y - sensors.LowerRight.Intersection.Value.Y, sensors.LowerLeft.Intersection.Value.X - sensors.LowerRight.Intersection.Value.X);
            //     transform.Rotation = angle * 180 / MathF.PI + 180;
            // }
            // else
            // {
            //     transform.Rotation = 0;
            // }

            var position = sensors.LowerRight.Position;
            var intersection = sonic.Facing == Facing.Left ? sensors.LowerLeft.Intersection ?? sensors.LowerRight.Intersection : sensors.LowerRight.Intersection ?? sensors.LowerLeft.Intersection;


            if (intersection.HasValue)
            {

                if (position.Y > intersection.Value.Y)
                {
                    transform.Position = new Vector2f(transform.Position.X, intersection.Value.Y - 48 / 2 - 1);
                    sonic.IsOnGround = true;
                }
                else
                {
                    sonic.IsOnGround = Math.Abs(intersection.Value.Y - position.Y) > 2 ? false : true;
                }
            }
            else
            {
                sonic.IsOnGround = false;
            }




            if (sonic.State != SonicState.Charging && sonic.IsOnGround)
                sonic.State = SonicState.Idle;

            HandleCrouch(ref sonic, ref velocity);
            HandleSpinDashCharge(ref transform, ref velocity, ref sonic);
            HandleHorizontalMovement(ref velocity, ref sonic);
            HandleVerticalMovement(ref transform, ref velocity, ref sonic);

            if ((velocity.Speed.X > 0 && sensors.HorizontalRight.Distance < 2) || (velocity.Speed.X < 0 && sensors.HorizontalLeft.Distance < 2))
            {
                velocity.Speed = new Vector2f(0, velocity.Speed.Y);
                velocity.GroundSpeed = 0;
            }



            transform.Position = new Vector2f(transform.Position.X + velocity.Speed.X, transform.Position.Y + velocity.Speed.Y);

            // TODO - GET RID OF THIS WORK AROUND 
            // if (transform.Position.Y >= 30 * 16)
            // {
            //     transform.Position = new Vector2f(transform.Position.X, 30 * 16);
            //     sonic.IsOnGround = true;
            // }
            // else
            // {
            //     sonic.IsOnGround = false;
            // }

            if (velocity.Speed.X > 0)
                sonic.Facing = Facing.Right;
            else if (velocity.Speed.X < 0)
                sonic.Facing = Facing.Left;
            else if (velocity.Speed.X == 0 && Input.IsKeyPressed(Direction.Forward))
                sonic.Facing = Facing.Right;
            else if (velocity.Speed.X == 0 && Input.IsKeyPressed(Direction.Backward))
                sonic.Facing = Facing.Left;
        }

        private void HandleHorizontalMovement(ref Velocity velocity, ref Sonic sonic)
        {
            var forward = Input.IsKeyPressed(Direction.Forward);
            var backward = Input.IsKeyPressed(Direction.Backward);

            if (Math.Abs(velocity.GroundSpeed) > 0 && sonic.IsOnGround)
                sonic.State = SonicState.Running;

            if ((!backward && !forward) || (forward && backward))
            {
                velocity.GroundSpeed -= Math.Min(Math.Abs(velocity.GroundSpeed), FRICTION_SPEED) * Math.Sign(velocity.GroundSpeed);
            }

            else if (forward && sonic.State != SonicState.Charging)
            {
                if (velocity.GroundSpeed < 0) // going backwards
                {
                    sonic.State = SonicState.Skidding;
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

            else if (backward && sonic.State != SonicState.Charging)
            {
                if (velocity.GroundSpeed > 0) // going forwards
                {
                    sonic.State = SonicState.Skidding;
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


        }

        private void HandleVerticalMovement(ref Transform transform, ref Velocity velocity, ref Sonic sonic)
        {
            var forward = Input.IsKeyPressed(Direction.Forward);
            var backward = Input.IsKeyPressed(Direction.Backward);
            var space = Input.IsKeyStarted(Direction.Space);

            var vX = velocity.Speed.X;
            var vY = velocity.Speed.Y;

            if (sonic.IsOnGround)
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

            if (space && sonic.IsOnGround && sonic.State != SonicState.Charging && sonic.State != SonicState.Crouching)
            {
                vX -= JUMP_FORCE * MathF.Sin(transform.GroundAngle);
                vY -= JUMP_FORCE * MathF.Cos(transform.GroundAngle);
                sonic.State = SonicState.Jumping;
            }

            velocity.Speed = new Vector2f(vX, vY);
        }

        private void HandleSpinDashCharge(ref Transform transform, ref Velocity velocity, ref Sonic sonic)
        {
            var space = Input.IsKeyStarted(Direction.Space);
            var down = Input.IsKeyPressed(Direction.Down);

            if (space && sonic.IsOnGround && velocity.GroundSpeed == 0 && sonic.SpinRef == 0 && sonic.State == SonicState.Crouching)
                sonic.State = SonicState.Charging;

            if (sonic.State != SonicState.Charging)
                sonic.SpinRef = 0;

            if (sonic.SpinRef > 0)
                sonic.SpinRef -= sonic.SpinRef / 0.125f / 256;

            if (space && sonic.State == SonicState.Charging)
                sonic.SpinRef += 2;

            if (sonic.SpinRef > 8)
                sonic.SpinRef = 8;

            if (!down && sonic.State == SonicState.Charging)
            {
                var multiplier = sonic.Facing == Facing.Right ? 1 : -1;
                velocity.GroundSpeed = (8f + MathF.Floor(sonic.SpinRef) / 2f) * multiplier;
                sonic.State = SonicState.Running;
                sonic.SpinRef = 0;
            }
        }

        private void HandleCrouch(ref Sonic sonic, ref Velocity velocity)
        {
            var down = Input.IsKeyPressed(Direction.Down);

            if (down && sonic.IsOnGround && velocity.GroundSpeed == 0 && sonic.State != SonicState.Charging)
                sonic.State = SonicState.Crouching;

        }
    }
}
