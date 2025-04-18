namespace SonicRemake.Layout.Engine;

public abstract record Sizing
{
    public int Calculated { get; internal set; } = 0;
}

public record FitSizing : Sizing;
public record GrowSizing : Sizing;
public record FixedSizing : Sizing
{
    public FixedSizing(int Size)
    {
        Calculated = Size;
    }
}

public record Positioning
{
    public (int X, int Y) Calculated { get; internal set; } = (0, 0);

    public Position Type { get; internal set; } = Position.Relative;
}
