namespace SonicRemake.Layout;

public class FitSize : Size
{
  public int Fixed => int.MinValue;
  public int Min => int.MinValue;
  public int Max => int.MinValue;

  public FitSize()
  {
  }
}
