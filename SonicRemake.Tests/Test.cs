using FluentAssertions;
using SFML.Graphics;
using SonicRemake.Layout;

namespace SonicRemake.Tests;

public class Tests
{
    [Test]
    public void BreadthFirst()
    {
        var a = new Div("1");
        var b = new Div("2");
        var c = new Div("3");
        var d = new Div("4");
        var e = new Div("5");
        
        UI.Init(0 , 0);
        UI.Open(a);
        
        UI.Open(c);
        UI.Close();
        
        UI.Open(d);
        UI.Close();
        UI.Close();
        
        UI.Open(b);
        UI.Open(e);
        UI.Close();
        UI.Close();
        
        var result = UI.BreadthFirst().Select(div => div.Id).ToArray();
        result.Should().NotBeNullOrEmpty();
        result.Should().HaveCount(6);
        result.Should().ContainInOrder("ROOT", "1", "2", "3", "4", "5");
    }
    
    [Test]
    public void ReverseBreadthFirst()
    {
        var a = new Div("1");
        var b = new Div("2");
        var c = new Div("3");
        var d = new Div("4");
        var e = new Div("5");
        
        UI.Init(0 , 0);
        UI.Open(a);
        
        UI.Open(c);
        UI.Close();
        
        UI.Open(d);
        UI.Close();
        UI.Close();
        
        UI.Open(b);
        UI.Open(e);
        UI.Close();
        UI.Close();
        
        var result = UI.ReverseBreadthFirst().Select(div => div.Id).ToArray();
        result.Should().NotBeNullOrEmpty();
        result.Should().HaveCount(6);
        result.Should().ContainInOrder("5", "4", "3", "2", "1", "ROOT");
    }
    
    [Test]
    public void FixedSize()
    {
        var div = new Div("test").Size(20, 40);
        
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
        var a = new Div().Size(10, 10);
        var b = new Div().Size(10, 10);
        
        UI.Init(1000, 1000); 
        UI.Open(parent);
        UI.Open(a);
        UI.Close();
        UI.Open(b);
        UI.Close();
        UI.Close();
        UI.Calculate();
        
        Assert.That(parent.Width.Calculated, Is.EqualTo(20));
    }
}