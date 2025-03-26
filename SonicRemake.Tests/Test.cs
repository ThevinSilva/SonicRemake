using FluentAssertions;
using SFML.Graphics;
using SonicRemake.Layout;

namespace SonicRemake.Tests;

public class Tests
{
    [Test]
    public void BreadthFirst()
    {
        var a = new Node("1");
        var b = new Node("2");
        var c = new Node("3");
        var d = new Node("4");
        var e = new Node("5");

        UI.Init(0, 0);
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
        var a = new Node("1");
        var b = new Node("2");
        var c = new Node("3");
        var d = new Node("4");
        var e = new Node("5");

        UI.Init(0, 0);
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
        var div = new Node("test").Size(20, 40);

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
        var a = new Node("a").Size(Size.Fit);
        var b = new Node("b").Size(Size.Fit);
        var c = new Node("c").Size(Size.Fit);
        var d = new Node("d").Size(10);

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
        var a = new Node("a").Size(Size.Fit);
        var b = new Node("b").Size(Size.Fit);
        var c = new Node("c").Size(10);
        var d = new Node("d").Size(10);

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
        var a = new Node("a").Size(Size.Fit).Gap(10);
        var b = new Node("b").Size(10);
        var c = new Node("c").Size(10);

        a.Children(b, c);

        UI.Init(int.MaxValue, int.MaxValue);
        UI.Open(a);
        UI.Close();
        UI.Calculate();

        a.Width.Calculated.Should().Be(30);
        a.Height.Calculated.Should().Be(10);
    }


    [Test]
    public void Padding()
    {
        var a = new Node("a").Size(Size.Fit).Padding(5);
        var b = new Node("b").Size(10);
        var c = new Node("c").Size(10);

        a.Children(b, c);

        UI.Init(int.MaxValue, int.MaxValue);
        UI.Open(a);
        UI.Close();
        UI.Calculate();

        a.Width.Calculated.Should().Be(30);
        a.Height.Calculated.Should().Be(20);
    }

    [Test]
    public void Grow()
    {
        var a = new Node("a").Size(100, 100);
        var b = new Node("b").Size(Size.Grow);

        a.Children(b);

        UI.Init(1000, 1000);
        UI.Open(a);
        UI.Close();
        UI.Calculate();

        a.Width.Calculated.Should().Be(100);
        a.Height.Calculated.Should().Be(100);
        b.Width.Calculated.Should().Be(100);
        b.Height.Calculated.Should().Be(100);
    }

    [Test]
    public void GrowMultipleChildren()
    {
        var a = new Node("a").Size(100);
        var b = new Node("b").Size(Size.Grow);
        var c = new Node("c").Size(Size.Grow);

        a.Children(b, c);

        UI.Init(1000, 1000);
        UI.Open(a);
        UI.Close();
        UI.Calculate();

        a.Width.Calculated.Should().Be(100);
        a.Height.Calculated.Should().Be(100);
        b.Width.Calculated.Should().Be(50);
        b.Height.Calculated.Should().Be(100);
        c.Width.Calculated.Should().Be(50);
        c.Height.Calculated.Should().Be(100);
    }

    // [Test]
    public void GrowMultipleChildrenDifferentStartingSizes()
    {
        var a = new Node("a").Size(100);
        var b = new Node("b").Size(Size.Grow);
        var c = new Node("c").Size(Size.Grow);
        var d = new Node("d").Size(50);

        a.Children(b, c);

        UI.Init(1000, 1000);
        UI.Open(a);
        UI.Close();
        UI.Calculate();

        a.Width.Calculated.Should().Be(100);
        a.Height.Calculated.Should().Be(100);
        b.Width.Calculated.Should().Be(50);
        b.Height.Calculated.Should().Be(100);
        c.Width.Calculated.Should().Be(50);
        c.Height.Calculated.Should().Be(100);
    }
}
