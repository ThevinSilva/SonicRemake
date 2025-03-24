using System;
using System.Collections.Generic;

namespace SonicRemake.Layout;

public static class UI
{
  private static Log _log = new(typeof(UI));
  
  private static Div? _current;

  public static void Init(int rootWidth, int rootHeight)
  {
    _current = new Div("ROOT").Size(rootWidth, rootHeight);
  }
  
  public static void Calculate()
  {
    foreach (var div in BreadthFirst())
    {
      _log.Debug($"Calculating {div.Id}");
    }
  }

  public static IEnumerable<Div> BreadthFirst(Action<Div>? action = null)
  {
    if (_current == null)
      yield break;
    
    var queue = new Queue<Div>([_current]);

    while (queue.Count > 0)
    {
      var div = queue.Dequeue();
      action?.Invoke(div);

      foreach (var child in div.Children)
        queue.Enqueue(child);

      yield return div;
    }
  }

  public static IEnumerable<Div> ReverseBreadthFirst(Action<Div>? action = null)
  {
    if (_current == null)
      yield break;
    
    var stack = new Stack<Div>([_current]);

    while (stack.Count > 0)
    {
      var div = stack.Pop();
      action?.Invoke(div);

      for (var i = div.Children.Count - 1; i >= 0; i--)
        stack.Push(div.Children[i]);

      yield return div;
    }
  }

  public static Div Open(Div div)
  {
    _log.Debug($"Opening {div.Id}");
    
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
