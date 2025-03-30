using System;
using System.Collections.Generic;
using System.Linq;
using SonicRemake.Layout.Engine;

namespace SonicRemake.Layout;

// ReSharper disable once InconsistentNaming
public static class UI
{
	private static readonly Log Log = new(typeof(UI));

	public static Node? Document { get; private set; }
	private static Node? _current;

	public static float Scale { get; set; } = 3f;

	public static void Init(uint rootWidth, uint rootHeight)
	{
		Document = new Node("__DOCUMENT__").Size((int)rootWidth, (int)rootHeight).Flow(Flow.Vertical).Position(Position.Absolute);
		_current = Document;
	}

	private static int Scaled(int value) => (int)Math.Round(value * Scale);

	public static void Calculate()
	{
		if (Document == null)
			return;

		// initial pass: scale fixed sizes
		foreach (var node in BreadthFirst())
		{
			// if the node's width/height are fixed, apply the scale factor
			if (!(node.Width is FitSizing || node.Width is GrowSizing))
				node.Width.Calculated = Scaled(node.Width.Calculated);
			if (!(node.Height is FitSizing || node.Height is GrowSizing))
				node.Height.Calculated = Scaled(node.Height.Calculated);
		}

		// Fit pass
		foreach (var div in ReverseBreadthFirst())
		{
			// Skip root
			if (div.Parent == null)
				continue;

			var parent = div.Parent!;

			if (div.Width is FitSizing)
				div.Width.Calculated += Scaled(div.Padding.Left) + Scaled(div.Padding.Right);

			if (div.Height is FitSizing)
				div.Height.Calculated += Scaled(div.Padding.Top + div.Padding.Bottom);

			if (parent.Axis is FitSizing)
			{
				var index = parent.Children.IndexOf(div);
				if (index > 0)
					parent.Axis.Calculated += Scaled(parent.Gap.Calculated);

				parent.Axis.Calculated += div.Axis.Calculated;
			}

			if (parent.CrossAxis is FitSizing)
				parent.CrossAxis.Calculated = Math.Max(div.CrossAxis.Calculated, parent.CrossAxis.Calculated);
		}

		// Grow pass
		foreach (var div in BreadthFirst())
		{
			if (div.Parent == null)
				continue;

			if (div.Position.Type == Position.Absolute)
			{
				div.Width.Calculated = div.Width is GrowSizing ? div.Parent.Width.Calculated - Scaled(div.Padding.Left) - Scaled(div.Padding.Right) : div.Width.Calculated;
				div.Height.Calculated = div.Height is GrowSizing ? div.Parent.Height.Calculated - Scaled(div.Padding.Top) - Scaled(div.Padding.Bottom) : div.Height.Calculated;
				continue;
			}

			var parent = div.Parent!;

			// Grow cross axis
			foreach (Node child in parent.Children.Where(x => x.CrossAxis is GrowSizing).Where(x => x.Position.Type != Position.Absolute))
				child.CrossAxis.Calculated += parent.CrossAxis.Calculated - child.CrossAxis.Calculated;

			// Grow axis
			var remaningWidth = parent.Axis.Calculated
				- parent.Padding.Left - Scaled(parent.Padding.Right)
				- parent.Children.Sum(c => c.Width.Calculated)
				- (parent.Children.Count - 1) * Scaled(parent.Gap.Calculated);

			var growables = parent.Children
				.Where(c => c.Axis is GrowSizing)
				.ToList();

			if (growables.Count == 0)
				continue;

			// Distribute space to growables
			while (remaningWidth > growables.Count)
			{
				int smallest = int.MaxValue;
				int secondSmallest = int.MaxValue;
				int spaceToAdd = remaningWidth;

				foreach (var child in growables)
				{
					if (child.Axis.Calculated < smallest)
					{
						secondSmallest = smallest;
						smallest = child.Axis.Calculated;
					}

					if (child.Axis.Calculated > smallest)
					{
						secondSmallest = Math.Min(secondSmallest, child.Axis.Calculated);
						spaceToAdd = secondSmallest - smallest;
					}
				}

				spaceToAdd = Math.Min(spaceToAdd, remaningWidth / growables.Count);

				foreach (var child in growables)
				{
					if (child.Axis.Calculated == smallest)
					{
						child.Axis.Calculated += spaceToAdd;
						remaningWidth -= spaceToAdd;
					}
				}
			}

			// Distribute remaining pixels
			foreach (var child in growables)
			{
				if (remaningWidth <= 0)
					break;

				child.Axis.Calculated += 1;
				remaningWidth--;
			}
		}

		// Positioning pass
		foreach (var div in BreadthFirst())
		{
			if (div.Parent == null)
				continue;

			var parent = div.Parent!;
			var position = div.Position.Calculated;

			var nthChild = parent.Children.ToList().IndexOf(div);

			if (div.Position.Type == Position.Absolute)
			{
				position.X = Scaled(parent.Padding.Left);
				position.Y = Scaled(parent.Padding.Top);
			}
			else if (div.Position.Type == Position.Relative)
			{
				position.X = parent.Position.Calculated.X + Scaled(parent.Padding.Left);
				position.Y = parent.Position.Calculated.Y + Scaled(parent.Padding.Top);

				var offsetX = parent.Align.Horizontal switch
				{
					Align.Start => 0,
					Align.Center => (parent.Width.Calculated - div.Width.Calculated) / 2,
					Align.End => parent.Width.Calculated - div.Width.Calculated,
					_ => throw new ArgumentOutOfRangeException()
				};

				var offsetY = parent.Align.Vertical switch
				{
					Align.Start => 0,
					Align.Center => (parent.Height.Calculated - div.Height.Calculated) / 2,
					Align.End => parent.Height.Calculated - div.Height.Calculated,
					_ => throw new ArgumentOutOfRangeException()
				};

				position.X += offsetX;
				position.Y += offsetY;

				if (parent.Flow == Flow.Horizontal)
				{
					position.X += parent.Children.TakeWhile(x => x != div).Sum(x => x.Width.Calculated);
					position.X += nthChild * parent.Gap.Calculated;
				}
				else
				{
					position.Y += parent.Children.TakeWhile(x => x != div).Sum(x => x.Height.Calculated);
					position.Y += nthChild * parent.Gap.Calculated;
				}
			}

			div.Position.Calculated = position;
		}
	}

	public static IEnumerable<Node> BreadthFirst()
	{
		if (Document == null)
			yield break;

		var queue = new Queue<Node>();
		queue.Enqueue(Document);

		while (queue.Count > 0)
		{
			var div = queue.Dequeue();
			yield return div;

			foreach (var child in div.Children)
				queue.Enqueue(child);

		}
	}

	public static IEnumerable<Node> ReverseBreadthFirst()
	{
		if (Document == null)
			yield break;

		var queue = new Queue<Node>();
		queue.Enqueue(Document);

		var stack = new Stack<Node>();
		while (queue.Count > 0)
		{
			var div = queue.Dequeue();
			stack.Push(div);

			foreach (var child in div.Children)
				queue.Enqueue(child);
		}

		while (stack.Count > 0)
		{
			yield return stack.Pop();
		}
	}

	public static Node Open(Node node)
	{
		if (node.Parent != null)
			throw new Exception("Div already has a parent");

		if (_current == null)
			return node;

		_current.Children(node);
		node.Parent = node.Position.Type == Position.Absolute ? Document : _current;

		_current = node;
		return node;
	}

	public static void Close()
	{
		if (_current == null)
			return;

		_current = _current.Parent;
	}
}
