using System;
using System.Collections.Generic;
using System.Linq;

namespace SonicRemake.Layout;

// ReSharper disable once InconsistentNaming
public static class UI
{
  private static readonly Log Log = new(typeof(UI));
  
  private static Div? _current;

  public static void Init(int rootWidth, int rootHeight)
  {
    _current = new Div("ROOT").Size(rootWidth, rootHeight);
  }
  
  public static void Calculate()
  {
    // fixed pass
    foreach (var div in BreadthFirst())
    {
      if (div.Width is FixedSize fixedWidth)
        div.Width.Calculated = fixedWidth.Size;
      
      if (div.Height is FixedSize fixedHeight)
        div.Height.Calculated = fixedHeight.Size;
    }
    
    // fit pass
    foreach (var div in ReverseBreadthFirst())
    {
      if (div.Width is FitSize)
        div.Width.Calculated = div.Children.Sum(child => child.Width.Calculated) + (div.Children.Count - 1) * div.Gap;
      
      if (div.Height is FitSize)
        div.Height.Calculated = div.Children.Max(child => child.Height.Calculated);
    }
    
    // grow pass
    foreach (var div in BreadthFirst())
    {
      if (div.Width is GrowSize)
        div.Width.Calculated = div.Parent.Width.Calculated - div.Parent.Padding.left - div.Parent.Padding.right;
      
      if (div.Height is GrowSize)
        div.Height.Calculated = div.Parent.Height.Calculated - div.Parent.Padding.top - div.Parent.Padding.bottom;
    }
  }

  public static IEnumerable<Div> BreadthFirst()
  {
    if (_current == null)
      yield break;
    
    var queue = new Queue<Div>([_current]);

    while (queue.Count > 0)
    {
      var div = queue.Dequeue();

      foreach (var child in div.Children)
        queue.Enqueue(child);

      yield return div;
    }
  }

  public static IEnumerable<Div> ReverseBreadthFirst()
  {
    if (_current == null)
      yield break;
    
    var stack = new Stack<Div>([_current]);

    while (stack.Count > 0)
    {
      var div = stack.Pop();

      for (var i = div.Children.Count - 1; i >= 0; i--)
        stack.Push(div.Children[i]);

      yield return div;
    }
  }

  public static Div Open(Div div)
  {
    if (div.Parent != null)
      throw new Exception("Div already has a parent");

    if (_current == null)
      throw new Exception("No current div to open. Did you forget to call Init?");
    
    div.Parent = _current;
    div.Parent.Children.Add(div);

    _current = div;
    return div;
  }

  public static void Close()
  {
    _current = _current?.Parent ?? throw new Exception("No parent to close. Did you forget to call Init?");
  }
}
