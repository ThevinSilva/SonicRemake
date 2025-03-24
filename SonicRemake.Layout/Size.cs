namespace SonicRemake.Layout;

public abstract record Size
{
    public int Calculated { get; internal set; } = 0;
}

public record FitSize : Size;
public record GrowSize : Size;
public record FixedSize : Size
{
    public FixedSize(int Size)
    {
        this.Size = Size;
        Calculated = Size;
    }

    public int Size { get; init; }
}
