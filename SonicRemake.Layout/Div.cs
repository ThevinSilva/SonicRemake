using System.Collections.Generic;
using SFML.Graphics;

namespace SonicRemake.Layout;

public class Div
{
  public Div parent;

  // null means fixed
  public Size Width { get; set; } = new FitSize();

  public Size Height { get; set; } = new FitSize();

  public Color Background { get; set; } = Color.Transparent;

  public Color Foreground { get; set; } = Color.White;

  public (int left, int top, int right, int bottom) Padding { get; set; }

  public int Gap { get; set; }

  public Flow Flow { get; set; } = Flow.LeftToRight;

  public List<Div> Children { get; set; } = [];
}

public enum Sizing
{
  Fit,
  Grow
}
