using System.Collections.Generic;
using SFML.Graphics;

namespace SonicRemake.Layout;

public class Div
{
  public Div(string? id = null)
  {
    Id = id;
  }
  
  public string? Id { get; internal set; }
  
  public Div? Parent;

  public Size Width { get; internal set; } = new FitSize();
  public Size Height { get; internal set; } = new FitSize();

  public Color Background { get; internal set; } = Color.Transparent;
  public Color Foreground { get; internal set; } = Color.White;

  public (int left, int top, int right, int bottom) Padding { get; internal set; }

  public int Gap { get; internal set; }

  public Flow Flow { get; internal set; } = Flow.LeftToRight;

  public IList<Div> Children { get; internal set; } = [];
}

public enum Sizing
{
  Fit,
  Grow
}
