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

	public static void Init(int rootWidth, int rootHeight)
	{
		_root = new Node("__ROOT__").Size(rootWidth, rootHeight);
		_current = _root;
	}

	public static void Calculate()
	{
		// Fit pass
		foreach (var div in ReverseBreadthFirst())
		{
			// Skip root
			if (div.Parent == null)
				continue;

			var parent = div.Parent!;

			// Padding
			div.Width.Calculated += div.Padding.Left + div.Padding.Right;
			div.Height.Calculated += div.Padding.Top + div.Padding.Bottom;

			// Gap
			div.Axis.Calculated += (div.Children.Count - 1) * div.Gap;

			if (parent.Axis is FitSizing)
				parent.Axis.Calculated += div.Axis.Calculated;

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
				- (parent.Children.Count - 1) * div.Gap;

			var growables = parent.Children
				.Where(c => c.Axis is GrowSizing)
				.ToList();

			if (growables.Count == 0)
				continue;

			while (remaningWidth > 0)
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
		}

		// Positioning pass
		foreach (var div in ReverseBreadthFirst())
		{
			if (div.Parent == null)
				continue;

			var parent = div.Parent!;

			int primaryOffset = parent.Flow == Flow.Horizontal
				   ? parent.Position.X + parent.Padding.Left
				   : parent.Position.Y + parent.Padding.Top;

			foreach (var child in parent.Children)
			{
				if (parent.Flow == Flow.Horizontal)
				{
					// set child's x position to the current primary offset
					// and center the child vertically within the parent's cross axis (height)
					child.Position = (
						primaryOffset,
						parent.Position.Y + parent.Padding.Top + ((parent.Height.Calculated - child.Height.Calculated) / 2)
					);
					primaryOffset += child.Width.Calculated + parent.Gap;
				}
				else // vertical flow
				{
					// set child's y position to the current primary offset
					// and center the child horizontally within the parent's cross axis (width)
					child.Position = (
						parent.Position.X + parent.Padding.Left + ((parent.Width.Calculated - child.Width.Calculated) / 2),
						primaryOffset
					);
					primaryOffset += child.Height.Calculated + parent.Gap;
				}
			}
		}
	}

	public static IEnumerable<Node> BreadthFirst()
	{
		if (_root == null)
			throw new Exception("No root div. Did you forget to call Init?");

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
			throw new Exception("No root div. Did you forget to call Init?");

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
			throw new Exception("No current div to open. Did you forget to call Init?");

		_current.Children(node);
		node.Parent = _current;

		_current = node;
		return node;
	}

	public static void Close()
	{
		_current = _current?.Parent ?? throw new Exception("No parent to close. Did you forget to call Init?");
	}
}
