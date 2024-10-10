using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SonicRemake;

[JsonConverter(typeof(StringEnumConverter))]
public enum Direction
{
    Up,
    Backward,
    Down,
    Forward,
    Space
}
