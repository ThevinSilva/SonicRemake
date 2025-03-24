using SFML.Graphics;
using SonicRemake.Layout;

namespace SonicRemake.Tests;

public class Tests
{
    [Test]
    public void FixedSize()
    {
        var div = new Div().Size(20, 40);
        
        UI.Init(1000, 1000);
        UI.Open(div);
        UI.Close();
        UI.Calculate();
        
        Assert.That(div.Width.Calculated, Is.EqualTo(20));
        Assert.That(div.Height.Calculated, Is.EqualTo(40));
    }
    
    [Test]
    public void FitSize()
    {
        var parent = new Div().Size(Sizing.Fit);
        var child = new Div().Size(10, 10);
        
        UI.Init(1000, 1000); 
        UI.Open(parent);
        UI.Open(child);
        UI.Close();
        UI.Close();
        UI.Calculate();
        
        Assert.That(parent.Width.Calculated, Is.EqualTo(10));
        Assert.That(parent.Height.Calculated, Is.EqualTo(10));
    }
    
    [Test]
    public void GrowSize()
    {
        var parent = new Div().Size(10, 10);
        var child = new Div().Size(Sizing.Grow);
        
        UI.Init(1000, 1000); 
        UI.Open(parent);
        UI.Open(child);
        UI.Close();
        UI.Close();
        UI.Calculate();
        
        Assert.That(child.Width.Calculated, Is.EqualTo(10));
        Assert.That(child.Height.Calculated, Is.EqualTo(10));
    }
}