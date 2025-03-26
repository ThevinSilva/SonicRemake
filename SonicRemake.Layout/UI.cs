using System;
using System.Collections.Generic;
using System.Linq;
using SFML.System;

namespace SonicRemake.Layout;

// ReSharper disable once InconsistentNaming
public static class UI
{
	private static readonly Log Log = new(typeof(UI));

	private static Node? _root;
	private static Node? _current;

	public static void Init(uint rootWidth, uint rootHeight)
	{
		_root = new Div("__ROOT__").Size((int)rootWidth, (int)rootHeight).Flow(Flow.Vertical);
		_current = _root;
	}

	public static void Calculate()
	{
		if (_root == null)
			return;

		// Fit pass
		foreach (var div in ReverseBreadthFirst())
		{
			// Skip root
			if (div.Parent == null)
				continue;

			var parent = div.Parent!;

			if (div.Width is FitSizing)
				div.Width.Calculated += div.Padding.Left + div.Padding.Right;

			if (div.Height is FitSizing)
				div.Height.Calculated += div.Padding.Top + div.Padding.Bottom;

			if (parent.Axis is FitSizing)
			{
				var index = parent.Children.IndexOf(div);
				if (index > 0)
					parent.Axis.Calculated += parent.Gap.Calculated;

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

			var parent = div.Parent!;

			// Grow cross axis
			foreach (Node child in parent.Children.Where(x => x.CrossAxis is GrowSizing))
				child.CrossAxis.Calculated += parent.CrossAxis.Calculated - child.CrossAxis.Calculated;

			// Grow axis
			var remaningWidth = parent.Axis.Calculated
				- parent.Padding.Left - parent.Padding.Right
				- parent.Children.Sum(c => c.Width.Calculated)
				- (parent.Children.Count - 1) * parent.Gap.Calculated;

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
			var position = div.Position;

			var nthChild = parent.Children.IndexOf(div);

			position.X = parent.Position.X + parent.Padding.Left;
			position.Y = parent.Position.Y + parent.Padding.Top;

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

			div.Position = position;
		}
	}

	public static IEnumerable<Node> BreadthFirst()
	{
		if (_root == null)
			yield break;

		var queue = new Queue<Node>();
		queue.Enqueue(_root);

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
		if (_root == null)
			yield break;

		var queue = new Queue<Node>();
		queue.Enqueue(_root);

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
		node.Parent = _current;

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
