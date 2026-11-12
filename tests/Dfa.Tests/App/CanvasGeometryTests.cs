namespace Dfa.Tests.App;

using Avalonia;
using Dfa.App.Controls;
using Shouldly;

public class CanvasGeometryTests
{
    private static readonly Point _a = new(100, 100);
    private static readonly Point _b = new(300, 160);

    private static double Distance(Point p, Point q) => ((Vector)(p - q)).Length;

    [Fact]
    public void Straight_StartsAndEndsOnTheCircles_AlongTheCenterLine()
    {
        var edge = CanvasGeometry.Straight(_a, _b);

        Distance(edge.Start, _a).ShouldBe(CanvasGeometry.Radius, 1e-9);
        Distance(edge.End, _b).ShouldBe(CanvasGeometry.Radius, 1e-9);
        (Distance(_a, edge.Start) + Distance(edge.Start, edge.End) + Distance(edge.End, _b))
            .ShouldBe(Distance(_a, _b), 1e-9);
    }

    [Fact]
    public void Curved_BothDirections_EndOnTheCircles_AndBendToOppositeSides()
    {
        var there = CanvasGeometry.Curved(_a, _b);
        var back = CanvasGeometry.Curved(_b, _a);

        Distance(there.Start, _a).ShouldBe(CanvasGeometry.Radius, 1e-9);
        Distance(there.End, _b).ShouldBe(CanvasGeometry.Radius, 1e-9);
        Side(there.Label).ShouldNotBe(Side(back.Label));
        Side(there.Control1).ShouldBe(Side(there.Label));
    }

    [Fact]
    public void PassesThrough_OnlyForStatesNearTheSegment()
    {
        var from = new Point(0, 0);
        var to = new Point(400, 0);

        CanvasGeometry.PassesThrough(from, to, new Point(200, 10)).ShouldBeTrue();
        CanvasGeometry.PassesThrough(from, to, new Point(200, 60)).ShouldBeFalse();
        CanvasGeometry.PassesThrough(from, to, new Point(600, 0)).ShouldBeFalse();
    }

    [Fact]
    public void Loop_IsAboveTheState_AndTouchesTheCircle()
    {
        var loop = CanvasGeometry.Loop(_a);

        Distance(loop.Start, _a).ShouldBe(CanvasGeometry.Radius, 1e-6);
        Distance(loop.End, _a).ShouldBe(CanvasGeometry.Radius, 1e-6);
        loop.Label.Y.ShouldBeLessThan(_a.Y - CanvasGeometry.Radius);
    }

    [Fact]
    public void ArrowHead_TipIsTheEnd_AndPointsAlongTheArrow()
    {
        var edge = CanvasGeometry.Straight(_a, _b);

        var (tip, left, right) = CanvasGeometry.ArrowHead(edge);

        tip.ShouldBe(edge.End);
        Distance(left, _a).ShouldBeLessThan(Distance(tip, _a));
        Distance(left, tip).ShouldBe(Distance(right, tip), 1e-9);
    }

    [Fact]
    public void Straight_BetweenStatesOnTheSameSpot_StaysFinite()
    {
        var edge = CanvasGeometry.Straight(_a, _a);

        double.IsFinite(edge.End.X).ShouldBeTrue();
        double.IsFinite(CanvasGeometry.ArrowHead(edge).Left.Y).ShouldBeTrue();
    }

    [Fact]
    public void HitsState_InsideTheCircleOnly()
    {
        CanvasGeometry.HitsState(_a, _a + new Vector(CanvasGeometry.Radius - 1, 0)).ShouldBeTrue();
        CanvasGeometry.HitsState(_a, _a + new Vector(CanvasGeometry.Radius + 1, 0)).ShouldBeFalse();
    }

    [Fact]
    public void HitsEdge_NearTheCurveOrTheLabel_NotFarAway()
    {
        var edge = CanvasGeometry.Curved(_a, _b);

        CanvasGeometry.HitsEdge(edge, edge.Start).ShouldBeTrue();
        CanvasGeometry.HitsEdge(edge, edge.End + new Vector(0, 3)).ShouldBeTrue();
        CanvasGeometry.HitsEdge(edge, edge.Label).ShouldBeTrue();
        CanvasGeometry.HitsEdge(edge, new Point(_a.X, _b.Y + 100)).ShouldBeFalse();
    }

    // Which side of the line a→b a point lies on (sign of the cross product).
    private static int Side(Point p) => Math.Sign(((_b.X - _a.X) * (p.Y - _a.Y)) - ((_b.Y - _a.Y) * (p.X - _a.X)));
}
