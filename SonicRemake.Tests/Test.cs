using SFML.Graphics;
using SonicRemake.Layout;

namespace SonicRemake.Tests;

public class Tests
{
    [Test]
    public void Test()
    {
        var parent = new Div("parent");
        var child = new Div("child").Size(300, 300);
        
        UI.Init(1000, 1000); 
        
        UI.Open(parent);
        UI.Open(child);
        UI.Close();
        UI.Close();
        
        UI.Calculate();
        
        Assert.That(child.Width.Calculated, Is.EqualTo(300));
    }
}