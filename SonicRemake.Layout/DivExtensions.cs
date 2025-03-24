using System;
using SFML.Graphics;

namespace SonicRemake.Layout;

public static class DivExtensions
{
  public static Div Size(this Div div, int width, int height)
  {
    div.Width = new FixedSizing(width);
    div.Height = new FixedSizing(height);

    return div;
  }
  
  public static Div Size(this Div div, int size)
  {
    div.Width = new FixedSizing(size);
    div.Height = new FixedSizing(size);
    return div;
  }

  public static Div Size(this Div div, Size sizing)
  {
    div.Width = sizing == Layout.Size.Grow ? new GrowSizing() : new FitSizing();
    div.Height = sizing == Layout.Size.Grow ? new GrowSizing() : new FitSizing();
    return div;
  }
  
  public static Div Size(this Div div, int width, Size height)
  {
    div.Width = new FixedSizing(width);
    div.Height = height == Layout.Size.Grow ? new GrowSizing() : new FitSizing();
    return div;
  }

  public static Div Size(this Div div, Size width, int height)
  {
    div.Width = width == Layout.Size.Grow ? new GrowSizing() : new FitSizing();
    div.Height = new FixedSizing(height);
    return div;
  }

  public static Div Size(this Div div, Size width, Size height)
  {
    div.Width = width == Layout.Size.Grow ? new GrowSizing() : new FitSizing();
    div.Height = height == Layout.Size.Grow ? new GrowSizing() : new FitSizing();
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
  
  public static Div Children(this Div div, params Div[] children)
  {
    foreach (var child in children)
    {
      if (child.Parent != null)
        throw new Exception("Div already has a parent");
      
      div.Children.Add(child);
      child.Parent = div;
    }

    return div;
  }
}
