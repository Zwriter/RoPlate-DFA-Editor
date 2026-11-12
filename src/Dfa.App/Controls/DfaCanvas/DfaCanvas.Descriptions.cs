namespace Dfa.App.Controls;

using System.Globalization;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Dfa.Core.Documents;

// E-07 (SI-10): what each state "remembers", as small gray text below its circle,
// edited from the state's context menu ("Description…") with SetDescription.
public sealed partial class DfaCanvas
{
    private const double DescriptionFontSize = 11;
    private const double DescriptionWidth = 140;
    private const double DescriptionGap = 6;

    // Room below the lowest states for their descriptions (two or three short lines).
    private const double DescriptionRoom = 48;

    private static void DrawDescription(DrawingContext context, IBrush brush, string text, Point center)
    {
        var formatted = new FormattedText(
            text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, Typeface.Default, DescriptionFontSize, brush)
        {
            MaxTextWidth = DescriptionWidth,
            TextAlignment = TextAlignment.Center,
        };
        var top = center.Y + CanvasGeometry.Radius + DescriptionGap;
        context.DrawText(formatted, new Point(center.X - (DescriptionWidth / 2), top));
    }

    private MenuItem DescriptionMenuItem(DfaDocument document, string state)
    {
        var item = new MenuItem { Header = "Description…" };
        item.Click += async (_, _) => await DescribeAsync(document, state);
        return item;
    }

    // An empty description removes it (SetDescription).
    private async Task DescribeAsync(DfaDocument document, string state)
    {
        var current = document.Descriptions.TryGetValue(state, out var text) ? text : "";
        if (Dialogs() is { } dialogs && await dialogs.AskTextAsync("Description", state, current) is { } description)
        {
            Report(document.SetDescription(state, description.Trim()));
        }
    }
}
