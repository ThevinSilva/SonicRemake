using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


[JsonConverter(typeof(StringEnumConverter))]
public enum Direction
{
    Up,
    Backward,
    Down,
    Forward,
    Space
}
