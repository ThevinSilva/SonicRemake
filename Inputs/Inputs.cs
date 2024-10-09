using Newtonsoft.Json;
using SFML.Window;
namespace SonicRemake.Inputs;



public static class InputSystem
{

    public static Dictionary<Direction, Keyboard.Key[]> Map = new()
        {
            {Direction.Up , [Keyboard.Key.W]},
            {Direction.Backward, [Keyboard.Key.A]},
            {Direction.Down, [Keyboard.Key.S] },
            {Direction.Forward,[Keyboard.Key.D]},
            {Direction.Space, [Keyboard.Key.Space]},

        };


    public static HashSet<Direction> HandleInput()
    {
        HashSet<Direction> directions = [];

        foreach (Direction input in Map.Keys)
        {
            foreach (Keyboard.Key key in Map[input])
            {
                if (Keyboard.IsKeyPressed(key))
                {
                    directions.Add(input);
                }
            }
        }

        Console.WriteLine(JsonConvert.SerializeObject(directions));

        return directions;
    }
}
