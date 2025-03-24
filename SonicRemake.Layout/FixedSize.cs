namespace SonicRemake.Layout;

public class FixedSize : Size
{
  public int Fixed { get; }
  public int Min => Fixed;
  public int Max => Fixed;

  public FixedSize(int fixedSize)
  {
    Fixed = fixedSize;
  }
}
