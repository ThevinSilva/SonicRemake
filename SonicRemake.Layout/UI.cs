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
    foreach (var div in ReverseBreadthFirst())
    {
      // Skip root
      if (div.Parent == null)
        continue;

      var parent = div.Parent!;
      
      var axis = div.Flow == Flow.Horizontal ? div.Width : div.Height;
      var crossAxis = div.Flow == Flow.Horizontal ? div.Height : div.Width;
      
      var parentAxis = parent.Flow == Flow.Horizontal ? parent.Width : parent.Height;
      var parentCrossAxis = parent.Flow == Flow.Horizontal ? parent.Height : parent.Width;
      
      div.Width.Calculated += div.Padding.Left + div.Padding.Right;
      div.Height.Calculated += div.Padding.Top + div.Padding.Bottom;

      var childGap = (div.Children.Count - 1) * div.Gap;
      
      axis.Calculated += childGap;
      
      if (parentAxis is FitSizing)
      {
        parentAxis.Calculated += axis.Calculated;
      }

      if (parentCrossAxis is FitSizing)
      {
        parentCrossAxis.Calculated = Math.Max(crossAxis.Calculated, parentCrossAxis.Calculated);
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
