using System.Drawing;

namespace SonicRemake.Layout;

public static class Layout
{
  private static Div root = new Div().Size(Sizing.Grow, Sizing.Grow);

  private static Div current = root;

  public static void Clear()
  {
    current = root;
  }

  public static IEnumerable<Div> BreadthFirst()
  {
    var queue = new Queue<Div>();
    queue.Enqueue(root);

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
    var stack = new Stack<Div>();
    var queue = new Queue<Div>();
    queue.Enqueue(root);

    while (queue.Count > 0)
    {
      var div = queue.Dequeue();
      stack.Push(div);

      foreach (var child in div.Children)
        queue.Enqueue(child);
    }

    while (stack.Count > 0)
      yield return stack.Pop();
  }

  public static Div Open(Div div)
  {
    if (div.parent != null)
      throw new Exception("Div already has a parent");

    div.parent = current;
    current.Children.Add(div);
    current = div;
    return div;
  }

  public static void Close()
  {
    if (current.parent == null)
      throw new Exception("No parent to close");

    current = current.parent;
  }
}
