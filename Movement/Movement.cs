namespace SonicRemake.Movement;

public class Movement
{
    private const float ACCELERATION_SPEED = 0.046875f;
    private const float DECELERATION_SPEED = 0.5f;
    private const float FRICTION_SPEED = ACCELERATION_SPEED;
    private const float TOP_SPEED = 6;

    public Direction CurrentDirrection { get; set;}
    public float GroundSpeed { private set; get; }

    public Movement()
    {
        GroundSpeed = 0;
    }

    public void HandleMovement(HashSet<Direction> inputs) 
    {
        if (GroundSpeed > 0 && CurrentDirrection != Direction.Forward) CurrentDirrection = Direction.Forward;
        if (GroundSpeed < 0 && CurrentDirrection != Direction.Backward) CurrentDirrection = Direction.Backward;


        if (inputs.Contains(Direction.Up)) 
        {
            // look up 
        };

        if (inputs.Contains(Direction.Forward))
        {
            GroundSpeed += GroundSpeed <= 0 ? DECELERATION_SPEED : ACCELERATION_SPEED;
        }

        if (inputs.Contains(Direction.Backward))
        {
            GroundSpeed -= GroundSpeed >= 0 ? DECELERATION_SPEED : ACCELERATION_SPEED;
        }


    }
}
