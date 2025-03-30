using FluentAssertions;
using SonicRemake.Layout;
using SonicRemake.Layout.Engine;

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

        var result = UI.BreadthFirst().Select(div => div.Id).Skip(1).ToArray();
        result.Should().NotBeNullOrEmpty();
        result.Should().HaveCount(5);
        result.Should().ContainInOrder("1", "2", "3", "4", "5");
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

        var result = UI.ReverseBreadthFirst().Select(div => div.Id).SkipLast(1).ToArray();
        result.Should().NotBeNullOrEmpty();
        result.Should().HaveCount(5);
        result.Should().ContainInOrder("5", "4", "3", "2", "1");
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

    [Test]
    public void GrowMultipleChildrenGap()
    {
        var a = new Node("a").Size(100).Gap(10);
        var b = new Node("b").Size(Size.Grow);
        var c = new Node("c").Size(Size.Grow);

        a.Children(b, c);

        UI.Init(1000, 1000);
        UI.Open(a);
        UI.Close();
        UI.Calculate();

        a.Width.Calculated.Should().Be(100);
        a.Height.Calculated.Should().Be(100);
        b.Width.Calculated.Should().Be(45);
        b.Height.Calculated.Should().Be(100);
        c.Width.Calculated.Should().Be(45);
        c.Height.Calculated.Should().Be(100);
    }

    [Test]
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

    [Test]
    public void PositionNoChildren()
    {
        var a = new Node("a").Size(100, 100);

        UI.Init(1000, 1000);
        UI.Open(a);
        UI.Close();
        UI.Calculate();

        a.Position.Calculated.Should().Be((0, 0));
    }

    [Test]
    public void PositionOneChild()
    {
        var a = new Node("a").Size(100, 100);
        var b = new Node("b").Size(50, 50);

        a.Children(b);

        UI.Init(1000, 1000);
        UI.Open(a);
        UI.Close();
        UI.Calculate();

        a.Position.Calculated.Should().Be((0, 0));
        b.Position.Calculated.Should().Be((0, 0));
    }

    [Test]
    public void PositionTwoChildren()
    {
        var a = new Node("a").Size(100, 100);
        var b = new Node("b").Size(50, 50);
        var c = new Node("c").Size(50, 50);

        a.Children(b, c);

        UI.Init(1000, 1000);
        UI.Open(a);
        UI.Close();
        UI.Calculate();

        a.Position.Calculated.Should().Be((0, 0));
        b.Position.Calculated.Should().Be((0, 0));
        c.Position.Calculated.Should().Be((50, 0));
    }

    [Test]
    public void PositionTwoChildrenGap()
    {
        var a = new Node("a").Size(100, 100).Gap(10);
        var b = new Node("b").Size(50, 50);
        var c = new Node("c").Size(50, 50);

        a.Children(b, c);

        UI.Init(1000, 1000);
        UI.Open(a);
        UI.Close();
        UI.Calculate();

        a.Position.Calculated.Should().Be((0, 0));
        b.Position.Calculated.Should().Be((0, 0));
        c.Position.Calculated.Should().Be((60, 0));
    }

    [Test]
    public void PositionTwoChildrenGapPadding()
    {
        var a = new Node("a").Size(100, 100).Gap(10).Padding(5);
        var b = new Node("b").Size(50, 50);
        var c = new Node("c").Size(50, 50);

        a.Children(b, c);

        UI.Init(1000, 1000);
        UI.Open(a);
        UI.Close();
        UI.Calculate();

        a.Position.Calculated.Should().Be((0, 0));
        b.Position.Calculated.Should().Be((5, 5));
        c.Position.Calculated.Should().Be((65, 5));
    }

    [Test]
    public void PositionTwoChildrenGrow()
    {
        var a = new Node("a").Size(100, 100).Gap(10);
        var b = new Node("b").Size(Size.Grow, Size.Grow);
        var c = new Node("c").Size(Size.Grow, Size.Grow);

        a.Children(b, c);

        UI.Init(1000, 1000);
        UI.Open(a);
        UI.Close();
        UI.Calculate();

        a.Position.Calculated.Should().Be((0, 0));
        b.Position.Calculated.Should().Be((0, 0));
        c.Position.Calculated.Should().Be((55, 0));
    }

    [Test]
    [Ignore("This test is not working as expected")]
    public void PositionStatic()
    {
        var a = new Node("a").Size(Size.Grow);
        var b = new Node("b").Size(Size.Grow);
        var c = new Node("c").Size(Size.Grow).Position(Position.Absolute);

        a.Children(b, c);

        UI.Init(10, 10);
        UI.Open(a);
        UI.Close();
        UI.Calculate();

        a.Position.Calculated.Should().Be((0, 0));
        a.Width.Calculated.Should().Be(10);
        a.Height.Calculated.Should().Be(10);
        b.Position.Calculated.Should().Be((0, 0));
        b.Width.Calculated.Should().Be(10);
        b.Height.Calculated.Should().Be(10);
        c.Position.Calculated.Should().Be((0, 0));
        c.Width.Calculated.Should().Be(10);
        c.Height.Calculated.Should().Be(10);
    }
}
