namespace SonicRemake.Layout;

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
        this.Size = Size;
        Calculated = Size;
    }

    public int Size { get; init; }
}
