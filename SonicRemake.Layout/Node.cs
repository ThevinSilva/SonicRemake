using System.Collections.Generic;
using SFML.Graphics;

namespace SonicRemake.Layout;

public class Node(string? id = null)
{
  public string? Id { get; } = id;

  public Node? Parent;

  public Sizing Width { get; internal set; } = new FitSizing();
  public Sizing Height { get; internal set; } = new FitSizing();

  public Color Background { get; internal set; } = Color.Transparent;
  public Color Foreground { get; internal set; } = Color.White;

  public (int Left, int Top, int Right, int Bottom) Padding { get; internal set; }

  public int Gap { get; internal set; }

  public Flow Flow { get; internal set; } = Flow.Horizontal;

  public IList<Node> Children { get; internal set; } = [];
}

public enum Size
{
  Fit,
  Grow
}
