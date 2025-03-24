namespace SonicRemake.Layout;

public class GrowSize : Size
{
  public int Fixed => int.MinValue;
  public int Min => int.MinValue;
  public int Max => int.MinValue;

  public GrowSize()
  {
  }
}
