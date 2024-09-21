using SFML.Window;
namespace SonicRemake.Inputs;



public class Inputs
{
    Dictionary<Direction, Keyboard.Key[]> Map { get; set; }

    public Inputs()
    {
        Map = new()
        { 
            {Direction.Up , [Keyboard.Key.W]}, 
            {Direction.Backward, [Keyboard.Key.A]},
            {Direction.Down, [Keyboard.Key.S] },
            {Direction.Forward,[Keyboard.Key.D]},
            {Direction.Space, [Keyboard.Key.Space]},

        };
    }

    public HashSet<Direction> HandleInput() 
    {
        HashSet<Direction> dirrections = [];

        foreach(Direction input in this.Map.Keys)
        {
            foreach (Keyboard.Key key in Map[input]) 
            {
                if (Keyboard.IsKeyPressed(key)) 
                {
                    dirrections.Add(input);
                }
            }
        }

        return dirrections;
    } 
}
