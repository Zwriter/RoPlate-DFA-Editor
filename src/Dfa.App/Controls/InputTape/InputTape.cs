namespace Dfa.App.Controls;

using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

// The input tape (S-02, SI-02): one cell per character, the cursor below the next symbol,
// symbols already read dimmed and the blocking cell marked.
public sealed class InputTape : UserControl
{
    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<InputTape, string>(nameof(Text), "");

    public static readonly StyledProperty<int> CursorPositionProperty =
        AvaloniaProperty.Register<InputTape, int>(nameof(CursorPosition));

    public static readonly StyledProperty<int?> ErrorPositionProperty =
        AvaloniaProperty.Register<InputTape, int?>(nameof(ErrorPosition));

    private const double CellSize = 32;
    private const double CursorHeight = 12;
    private const double SymbolFontSize = 16;

    static InputTape()
    {
        AffectsRender<InputTape>(TextProperty, CursorPositionProperty, ErrorPositionProperty);
        AffectsMeasure<InputTape>(TextProperty);
    }

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    // Index of the next symbol: Steps[Position - 1].Position + 1, or 0 (contract 3, section 6).
    public int CursorPosition
    {
        get => GetValue(CursorPositionProperty);
        set => SetValue(CursorPositionProperty, value);
    }

    public int? ErrorPosition
    {
        get => GetValue(ErrorPositionProperty);
        set => SetValue(ErrorPositionProperty, value);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var foreground = Foreground ?? Brushes.Black;
        var read = ThemeBrush.Find(this, "Brush.TapeRead", foreground);
        var cellPen = new Pen(foreground, 1);

        for (var i = 0; i < Text.Length; i++)
        {
            var cell = new Rect(i * CellSize, 0, CellSize, CellSize);
            context.DrawRectangle(null, cellPen, cell);
            DrawCharacter(context, i < CursorPosition ? read : foreground, Text[i] == ' ' ? '␣' : Text[i], cell);
        }

        // The blocking cell: a thick outline, so it is not shown by color alone.
        if (ErrorPosition is { } error && error >= 0 && error < Text.Length)
        {
            var pen = new Pen(ThemeBrush.Find(this, "Brush.Rejected", foreground), 3);
            context.DrawRectangle(null, pen, new Rect(error * CellSize, 0, CellSize, CellSize).Deflate(1.5));
        }

        DrawCursor(context, ThemeBrush.Find(this, "Brush.TapeCursor", foreground));
    }

    // One extra cell of room, so the cursor can stand after the last symbol.
    protected override Size MeasureOverride(Size availableSize) =>
        new((Text.Length + 1) * CellSize, CellSize + CursorHeight);

    private void DrawCharacter(DrawingContext context, IBrush brush, char c, Rect cell)
    {
        var text = new FormattedText(
            c.ToString(), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface(FontFamily), SymbolFontSize, brush);
        context.DrawText(text, cell.Center - new Point(text.Width / 2, text.Height / 2));
    }

    private void DrawCursor(DrawingContext context, IBrush brush)
    {
        var x = (CursorPosition + 0.5) * CellSize;
        var top = CellSize + 2;
        var geometry = new PolylineGeometry(
            [new Point(x, top), new Point(x - 6, top + CursorHeight - 2), new Point(x + 6, top + CursorHeight - 2)],
            isFilled: true);
        context.DrawGeometry(brush, null, geometry);
    }
}
