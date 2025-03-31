using SFML.Window;

namespace SonicRemake.Inputs;

public static class Input
{
    private static Log _log = new(typeof(Input));

    private static HashSet<Keyboard.Key> Keys = new()
    {
        Keyboard.Key.W,
        Keyboard.Key.A,
        Keyboard.Key.S,
        Keyboard.Key.D,
        Keyboard.Key.Space,
        Keyboard.Key.Escape,
    };

    // Sets to store current and previous key states
    private static HashSet<Keyboard.Key> currentPressedKeys = [];
    private static HashSet<Keyboard.Key> previousPressedKeys = [];

    // Method to be called once per frame to update the input state
    public static void UpdateInputState()
    {
        // Store the current pressed keys in the previous state
        previousPressedKeys = [.. currentPressedKeys];

        // Clear and update the currently pressed keys
        currentPressedKeys.Clear();

            // Check all keys in the map and update the currentPressedKeys
            foreach (Keyboard.Key key in Keys)
            {
                if (Keyboard.IsKeyPressed(key))
                {
                    currentPressedKeys.Add(key);
                }
            }
        }
        
        public static bool IsAnyKeyPressed() => currentPressedKeys.Count > 0; 

        // Base method to check if a specific key is being held down
        public static bool IsKeyPressed(Keyboard.Key key)
        {
            return currentPressedKeys.Contains(key);
        }

    // Base method to check if a key started being pressed this frame (new press)
    public static bool IsKeyStarted(Keyboard.Key key)
    {
        return currentPressedKeys.Contains(key) && !previousPressedKeys.Contains(key);
    }

    // Base method to check if a key was released this frame
    public static bool IsKeyEnded(Keyboard.Key key)
    {
        return !currentPressedKeys.Contains(key) && previousPressedKeys.Contains(key);
    }

    public static Keyboard.Key DirectionToKey(Direction direction)
    {
        return direction switch
        {
            Direction.Up => Keyboard.Key.W,
            Direction.Forward => Keyboard.Key.D,
            Direction.Backward => Keyboard.Key.A,
            Direction.Down => Keyboard.Key.S,
            Direction.Space => Keyboard.Key.Space,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null),
        };
    }
}
