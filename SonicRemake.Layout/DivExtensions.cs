using System;
using SFML.Graphics;
using SonicRemake.Common;

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
		node.Background = new ColorTexturing(color);
		return node;
	}

	public static Node Background(this Node node, TextureHandle texture)
	{
		node.Background = new SpriteTexturing(texture);
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
		node.Align = (alignment, alignment);
		return node;
	}

	public static Node Align(this Node node, Align horizontal, Align vertical)
	{
		node.Align = (horizontal, vertical);
		return node;
	}

	public static Node Position(this Node node, Position position)
	{
		node.Position.Type = position;
		return node;
	}

	public static Node Border(this Node node, Color color, int thickness = 1)
	{
		node.Border.Color = color;
		node.Border.Thickness = thickness;
		return node;
	}


	public static Node Children(this Node node, params Node[] children)
	{
		foreach (var child in children)
		{
			if (child.Parent != null)
				throw new Exception("Div already has a parent");

			node._children.Add(child);
			child.Parent = child.Position.Type == Layout.Position.Absolute ? UI.Document : node;
		}

		return node;
	}
}

public static class TextExtensions
{
	public static Text Content(this Text text, string content)
	{
		text.Content = content;
		return text;
	}
}
