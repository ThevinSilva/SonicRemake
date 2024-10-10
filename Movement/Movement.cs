
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

        private QueryDescription Query = new QueryDescription().WithAll<Sonic, Transform>();

        public Vector2f Position { set; get; }

        public static byte Scale { set; get; }


        // Horizontal Movement Constants https://info.sonicretro.org/SPG:Running
        private const float ACCELERATION_SPEED = 0.046875f;
        private const float DECELERATION_SPEED = 0.5f;
        private const float FRICTION_SPEED = ACCELERATION_SPEED;
        private const float TOP_SPEED = 6;
        public float GroundSpeed { private set; get; }


        // Vertical Movement Constants https://info.sonicretro.org/SPG:Air_State 
        private const float AIR_ACCELERATION_SPEED = ACCELERATION_SPEED * 2;
        private const float GRAVITY = 0.21875f;
        private const float JUMP_FORCE = 6.5f;
        public bool OnGround { set; get; }
        public float XSpeed { set; get; }
        public float YSpeed { set; get; }
        public bool ControlLock { set; get; }
        public ushort GroundAngle { set; get; }

        private Direction[] inputs;

        public Movement(float xpos, float ypos, byte scale = 4)
        {
            GroundSpeed = 0;
            XSpeed = 0;
            Position = new Vector2f(xpos, ypos);
            Scale = scale;
        }

        public void HandleMovement(HashSet<Direction> inputs)
        {
            HandleHorizontalMovement(inputs.Contains(Direction.Backward), inputs.Contains(Direction.Forward));

            HandleVerticalMovement(
                inputs.Contains(Direction.Space),
                inputs.Contains(Direction.Backward),
                inputs.Contains(Direction.Forward)
            );


            Position = new Vector2f(Position.X + (XSpeed * Scale), Position.Y + (YSpeed * Scale));

            if (Position.Y >= 500) OnGround = true;
            else OnGround = false;


        }

        public void HandleHorizontalMovement(bool backward, bool forward)
        {

            if (forward)
            {
                if (GroundSpeed < 0) // going backwards
                {
                    GroundSpeed += DECELERATION_SPEED;

                    if (GroundSpeed >= 0)
                        GroundSpeed = DECELERATION_SPEED;
                }
                else if (GroundSpeed < TOP_SPEED)
                {
                    GroundSpeed += ACCELERATION_SPEED;

                    if (GroundSpeed >= TOP_SPEED)
                        GroundSpeed = TOP_SPEED; //impose top speed limit
                }
            }
            else if (backward)
            {
                if (GroundSpeed > 0) // going forwards
                {
                    GroundSpeed -= DECELERATION_SPEED;

                    if (GroundSpeed <= 0)
                        GroundSpeed = -DECELERATION_SPEED;
                }
                else if (GroundSpeed > -TOP_SPEED)
                {
                    GroundSpeed -= ACCELERATION_SPEED;

                    //impose top speed limit
                    if (GroundSpeed <= -TOP_SPEED)
                        GroundSpeed = -TOP_SPEED;
                }
            }
            // no horizontal movement
            else
            {
                GroundSpeed -= Math.Sign(GroundSpeed) * FRICTION_SPEED;
            }
        }

        public void HandleVerticalMovement(bool space, bool backward, bool forward)
        {

            if (OnGround)
            {
                XSpeed = GroundSpeed * (float)Math.Cos(GroundAngle);
                YSpeed = GroundSpeed * -(float)Math.Sin(GroundAngle);
            }
            else
            {
                // Air Drag Added
                if (YSpeed < 0 && YSpeed > -4)
                    XSpeed -= XSpeed / 0.125f / 256;

                // Gravity Force Added
                YSpeed = YSpeed > 16 ? 16 : YSpeed + GRAVITY;

                if (forward)
                    XSpeed += AIR_ACCELERATION_SPEED;

                if (backward)
                    XSpeed -= AIR_ACCELERATION_SPEED;
            }

            if (space && OnGround)
            {
                _log.Debug("bruhhh");
                XSpeed -= JUMP_FORCE * MathF.Sin(GroundAngle);
                YSpeed -= JUMP_FORCE * MathF.Cos(GroundAngle);
            }


        }

        public override void OnPhysics(World world, GameContext context)
        {
            _log.Debug(Position);
            world.Query(in Query, (Entity e, ref Sonic s, ref Transform t) =>
            {
                HandleMovement(InputSystem.HandleInput());
                t.Position = Position;
            });
        }


    }
}
