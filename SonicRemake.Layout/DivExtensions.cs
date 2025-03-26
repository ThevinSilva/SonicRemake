using System;
using SFML.Graphics;

namespace SonicRemake.Layout;

public static class DivExtensions
{
  public static Node Size(this Node node, int width, int height)
  {
    node.Width = new FixedSizing(width);
    node.Height = new FixedSizing(height);

    return node;
  }

  public static Node Size(this Node node, int size)
  {
    node.Width = new FixedSizing(size);
    node.Height = new FixedSizing(size);
    return node;
  }

  public static Node Size(this Node node, Size sizing)
  {
    node.Width = sizing == Layout.Size.Grow ? new GrowSizing() : new FitSizing();
    node.Height = sizing == Layout.Size.Grow ? new GrowSizing() : new FitSizing();
    return node;
  }

  public static Node Size(this Node node, int width, Size height)
  {
    node.Width = new FixedSizing(width);
    node.Height = height == Layout.Size.Grow ? new GrowSizing() : new FitSizing();
    return node;
  }

  public static Node Size(this Node node, Size width, int height)
  {
    node.Width = width == Layout.Size.Grow ? new GrowSizing() : new FitSizing();
    node.Height = new FixedSizing(height);
    return node;
  }

  public static Node Size(this Node node, Size width, Size height)
  {
    node.Width = width == Layout.Size.Grow ? new GrowSizing() : new FitSizing();
    node.Height = height == Layout.Size.Grow ? new GrowSizing() : new FitSizing();
    return node;
  }

  public static Node Background(this Node node, Color color)
  {
    node.Background = color;
    return node;
  }

  public static Node Foreground(this Node node, Color color)
  {
    node.Foreground = color;
    return node;
  }

  public static Node Padding(this Node node, int padding)
  {
    node.Padding = (padding, padding, padding, padding);
    return node;
  }

  public static Node Padding(this Node node, int left, int top, int right, int bottom)
  {
    node.Padding = (Left: left, Top: top, Right: right, Bottom: bottom);
    return node;
  }

  public static Node Padding(this Node node, int horizontal, int vertical)
  {
    node.Padding = (horizontal, vertical, horizontal, vertical);
    return node;
  }

  public static Node Gap(this Node node, int gap)
  {
    node.Gap = new FixedGapping(gap);
    return node;
  }

  public static Node Gap(this Node node, Gap gap)
  {
    node.Gap = new GrowGapping();
    return node;
  }

  public static Node Flow(this Node node, Flow flow)
  {
    node.Flow = flow;
    return node;
  }

  public static Node Align(this Node node, Align alignment)
  {
    node.Alignment = (alignment, alignment);
    return node;
  }

  public static Node Align(this Node node, Align horizontal, Align vertical)
  {
    node.Alignment = (horizontal, vertical);
    return node;
  }

  public static Node Children(this Node node, params Node[] children)
  {
    foreach (var child in children)
    {
      if (child.Parent != null)
        throw new Exception("Div already has a parent");

      node.Children.Add(child);
      child.Parent = node;
    }

    return node;
  }
}
