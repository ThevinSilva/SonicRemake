using SFML.Graphics;

namespace SonicRemake.Layout;

public static class DivExtensions
{
  public static Div Size(this Div div, int width, int height)
  {
    div.Width = new FixedSize(width);
    div.Height = new FixedSize(height);

    return div;
  }

  public static Div Size(this Div div, Sizing sizing)
  {
    div.Width = sizing == Sizing.Grow ? new GrowSize() : new FitSize();
    div.Height = sizing == Sizing.Grow ? new GrowSize() : new FitSize();
    return div;
  }
  
  public static Div Size(this Div div, int width, Sizing height)
  {
    div.Width = new FixedSize(width);
    div.Height = height == Sizing.Grow ? new GrowSize() : new FitSize();
    return div;
  }

  public static Div Size(this Div div, Sizing width, int height)
  {
    div.Width = width == Sizing.Grow ? new GrowSize() : new FitSize();
    div.Height = new FixedSize(height);
    return div;
  }

  public static Div Size(this Div div, Sizing width, Sizing height)
  {
    div.Width = width == Sizing.Grow ? new GrowSize() : new FitSize();
    div.Height = height == Sizing.Grow ? new GrowSize() : new FitSize();
    return div;
  }

  public static Div Background(this Div div, Color color)
  {
    div.Background = color;
    return div;
  }

  public static Div Foreground(this Div div, Color color)
  {
    div.Foreground = color;
    return div;
  }

  public static Div Padding(this Div div, int padding)
  {
    div.Padding = (padding, padding, padding, padding);
    return div;
  }

  public static Div Padding(this Div div, int left, int top, int right, int bottom)
  {
    div.Padding = (left, top, right, bottom);
    return div;
  }

  public static Div Padding(this Div div, int horizontal, int vertical)
  {
    div.Padding = (horizontal, vertical, horizontal, vertical);
    return div;
  }

  public static Div Gap(this Div div, int gap)
  {
    div.Gap = gap;
    return div;
  }

  public static Div Flow(this Div div, Flow flow)
  {
    div.Flow = flow;
    return div;
  }
}
