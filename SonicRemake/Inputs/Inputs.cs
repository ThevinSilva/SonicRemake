using SFML.Window;

namespace SonicRemake.Inputs
{
    public static class Input
    {
        private static Log _log = new(typeof(Input));

        // Mapping directions to keys
        public static Dictionary<Direction, Keyboard.Key[]> Map = new()
        {
            { Direction.Up, new Keyboard.Key[] { Keyboard.Key.W } },
            { Direction.Backward, new Keyboard.Key[] { Keyboard.Key.A } },
            { Direction.Down, new Keyboard.Key[] { Keyboard.Key.S } },
            { Direction.Forward, new Keyboard.Key[] { Keyboard.Key.D } },
            { Direction.Space, new Keyboard.Key[] { Keyboard.Key.Space } },
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
            foreach (var keys in Map.Values)
            {
                foreach (Keyboard.Key key in keys)
                {
                    if (Keyboard.IsKeyPressed(key))
                    {
                        currentPressedKeys.Add(key);
                    }
                }
            }
        }

        // Base method to check if a specific key is being held down
        public static bool IsKeyPressed(Keyboard.Key key)
        {
            return currentPressedKeys.Contains(key);
        }

        // Overload method to check if any key in a direction is being held down
        public static bool IsKeyPressed(Direction direction)
        {
            foreach (Keyboard.Key key in Map[direction])
            {
                if (IsKeyPressed(key))
                    return true;
            }
            return false;
        }

        // Base method to check if a key started being pressed this frame (new press)
        public static bool IsKeyStarted(Keyboard.Key key)
        {
            return currentPressedKeys.Contains(key) && !previousPressedKeys.Contains(key);
        }

        // Overload method to check if any key in a direction started being pressed this frame
        public static bool IsKeyStarted(Direction direction)
        {
            foreach (Keyboard.Key key in Map[direction])
            {
                if (IsKeyStarted(key))
                    return true;
            }
            return false;
        }

        // Base method to check if a key was released this frame
        public static bool IsKeyEnded(Keyboard.Key key)
        {
            return !currentPressedKeys.Contains(key) && previousPressedKeys.Contains(key);
        }

        // Overload method to check if any key in a direction was released this frame
        public static bool IsKeyEnded(Direction direction)
        {
            foreach (Keyboard.Key key in Map[direction])
            {
                if (IsKeyEnded(key))
                    return true;
            }
            return false;
        }
    }
}
