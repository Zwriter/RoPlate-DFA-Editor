namespace Dfa.Tests.App;

using Dfa.App.Controls;
using Shouldly;

// E-05: the verdict is never shown by color alone.
public class CanvasHighlightTests
{
    [Theory]
    [InlineData(HighlightKind.Current, "Brush.StateCurrent", null)]
    [InlineData(HighlightKind.Accepted, "Brush.Accepted", "✓")]
    [InlineData(HighlightKind.Rejected, "Brush.Rejected", "✗")]
    public void HighlightStyle_UsesTheSharedBrush_AndAMarkForTheVerdict(HighlightKind kind, string brush, string? mark)
    {
        DfaCanvas.HighlightStyle(kind).ShouldBe((brush, mark));
    }
}
