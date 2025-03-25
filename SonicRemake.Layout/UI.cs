using System;
using System.Collections.Generic;
using System.Linq;

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
		CalculateFitPass();
	}

	private static void CalculateFitPass()
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

			var remaningWidth = parent.Axis.Calculated
				- parent.Padding.Left - parent.Padding.Right
				- div.Children.Sum(c => c.Width.Calculated)
				- (div.Children.Count - 1) * div.Gap;

			foreach (Node child in parent.Children.Where(x => x.Axis is GrowSizing))
				child.Axis.Calculated += remaningWidth;

			foreach (Node child in parent.Children.Where(x => x.CrossAxis is GrowSizing))
				child.CrossAxis.Calculated += parent.CrossAxis.Calculated - child.CrossAxis.Calculated;
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
