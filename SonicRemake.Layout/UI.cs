using System;
using System.Collections.Generic;
using System.Linq;

namespace SonicRemake.Layout;

// ReSharper disable once InconsistentNaming
public static class UI
{
  private static readonly Log Log = new(typeof(UI));

  private static Div? _root;
  private static Div? _current;

  public static void Init(int rootWidth, int rootHeight)
  {
    _root = new Div("__ROOT__").Size(rootWidth, rootHeight);
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
      Log.Debug("Calculating div", div.Id);

      if (div.Parent == null)
        continue;
      
      if (div.Width is FixedSize)
        div.Parent.Width.Calculated += div.Width.Calculated;

      if (div.Height is FitSize)
        div.Height.Calculated = div.Children.Max(child => child.Height.Calculated);
    }
  }

  public static IEnumerable<Div> BreadthFirst()
  {
    if (_root == null)
      throw new Exception("No root div. Did you forget to call Init?");
    
    var queue = new Queue<Div>();
    queue.Enqueue(_root);

    while (queue.Count > 0)
    {
      var div = queue.Dequeue();
      yield return div;

      foreach (var child in div.Children)
        queue.Enqueue(child);
    }
  }

  public static IEnumerable<Div> ReverseBreadthFirst()
  {
    if (_root == null)
      throw new Exception("No root div. Did you forget to call Init?");
    
    var queue = new Queue<Div>();
    queue.Enqueue(_root);

    var stack = new Stack<Div>();
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

  public static Div Open(Div div)
  {
    if (div.Parent != null)
      throw new Exception("Div already has a parent");

    if (_current == null)
      throw new Exception("No current div to open. Did you forget to call Init?");
    
    _current.Children.Add(div);
    div.Parent = _current;
    
    _current = div;
    return div;
  }

  public static void Close()
  {
    _current = _current?.Parent ?? throw new Exception("No parent to close. Did you forget to call Init?");
  }
}
