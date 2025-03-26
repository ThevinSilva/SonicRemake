using System.Collections.Generic;
using System.Collections.Immutable;
using System.Numerics;
using SFML.Graphics;
using SFML.System;

namespace SonicRemake.Layout;

public abstract class Node(string? id = null)
{
  public string? Id { get; } = id;

  public Node? Parent;

  public (int X, int Y) Position { get; internal set; } = (0, 0);

  public Sizing Width { get; internal set; } = new FitSizing();
  public Sizing Height { get; internal set; } = new FitSizing();

  public Flow Flow { get; internal set; } = Flow.Horizontal;

  public Sizing Axis => Flow == Flow.Horizontal ? Width : Height;
  public Sizing CrossAxis => Flow == Flow.Horizontal ? Height : Width;

  public (Align Horizontal, Align Vertical) Align { get; internal set; } = (Layout.Align.Start, Layout.Align.Start);

  public Color Background { get; internal set; } = Color.Transparent;
  public Color Foreground { get; internal set; } = Color.White;

  public (int Left, int Top, int Right, int Bottom) Padding { get; internal set; }

  public Gapping Gap { get; internal set; } = new FixedGapping(0);

  public ImmutableList<Node> Children => [.. _children];
  internal List<Node> _children = new();

  public abstract bool WorthRendering { get; }
}

public class Div(string? id = null) : Node(id)
{
  public override bool WorthRendering => Background != Color.Transparent;
}

public class Text(string? id = null) : Node(id)
{
  public override bool WorthRendering => true;

  public string Content { get; set; } = string.Empty;
}

public enum Size
{
  Fit,
  Grow
}

public enum Gap
{
  Grow
}

public enum Align
{
  Start,
  Center,
  End
}
