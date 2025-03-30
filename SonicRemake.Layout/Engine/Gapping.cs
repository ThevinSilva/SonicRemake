namespace SonicRemake.Layout.Engine;

public abstract record Gapping
{
    public int Calculated { get; internal set; } = 0;
}

public record GrowGapping : Gapping;
public record FixedGapping : Gapping
{
    public FixedGapping(int Size)
    {
        Calculated = Size;
    }
}
