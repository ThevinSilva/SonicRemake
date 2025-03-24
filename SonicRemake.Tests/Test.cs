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
        result.Should().ContainInOrder("__ROOT__", "1", "2", "3", "4", "5");
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
        result.Should().ContainInOrder("5", "4", "3", "2", "1", "__ROOT__");
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
    public void FitSizeDeep()
    {
        var a = new Div("a").Size(Size.Fit);
        var b = new Div("b").Size(Size.Fit);
        var c = new Div("c").Size(Size.Fit);
        var d = new Div("d").Size(10);
        
        a.Children(b);
        b.Children(c);
        c.Children(d);

        UI.Init(int.MaxValue, int.MaxValue);
        UI.Open(a);
        UI.Close();
        UI.Calculate();

        a.Width.Calculated.Should().Be(10);
        a.Height.Calculated.Should().Be(10);
        b.Width.Calculated.Should().Be(10);
        b.Height.Calculated.Should().Be(10);
        c.Width.Calculated.Should().Be(10);
        c.Height.Calculated.Should().Be(10);
        d.Width.Calculated.Should().Be(10);
        d.Height.Calculated.Should().Be(10);
    }
    
    [Test]
    public void FitSizeWide()
    {
        var a = new Div("a").Size(Size.Fit);
        var b = new Div("b").Size(Size.Fit);
        var c = new Div("c").Size(10);
        var d = new Div("d").Size(10);
        
        a.Children(b, c, d);

        UI.Init(int.MaxValue, int.MaxValue);
        UI.Open(a);
        UI.Close();
        UI.Calculate();

        a.Width.Calculated.Should().Be(20);
        a.Height.Calculated.Should().Be(10);
    }

    [Test]
    public void Gap()
    {
        var a = new Div("a").Size(Size.Fit).Gap(10);
        var b = new Div("b").Size(10);
        var c = new Div("c").Size(10);
        
        a.Children(b, c);
        
        UI.Init(int.MaxValue, int.MaxValue);
        UI.Open(a);
        UI.Close();
        UI.Calculate();
        
        a.Width.Calculated.Should().Be(30);
        a.Width.Calculated.Should().Be(20);
    }
    
    
    [Test]
    public void Padding()
    {
        var a = new Div("a").Size(Size.Fit).Padding(5);
        var b = new Div("b").Size(10);
        var c = new Div("c").Size(10);
        
        a.Children(b, c);
        
        UI.Init(int.MaxValue, int.MaxValue);
        UI.Open(a);
        UI.Close();
        UI.Calculate();
        
        a.Width.Calculated.Should().Be(30);
        a.Height.Calculated.Should().Be(20);
    }
}