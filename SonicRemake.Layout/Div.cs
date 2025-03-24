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

  public Sizing Width { get; internal set; } = new FitSizing();
  public Sizing Height { get; internal set; } = new FitSizing();

  public Color Background { get; internal set; } = Color.Transparent;
  public Color Foreground { get; internal set; } = Color.White;

  public (int left, int top, int right, int bottom) Padding { get; internal set; }

  public int Gap { get; internal set; }

  public Flow Flow { get; internal set; } = Flow.Horizontal;

  public IList<Div> Children { get; internal set; } = [];
}

public enum Size
{
  Fit,
  Grow
}
