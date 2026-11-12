namespace Dfa.App.Controls;

using System;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Dfa.Core.Documents;
using Dfa.Core.Model;

// E-08 (ED-12): the drawing saved as a PNG with RenderTargetBitmap, black on white whatever the app's theme.
public static class DfaCanvasImage
{
    private const double Dpi = 96;

    public static OpResult SavePng(DfaDocument document, string path)
    {
        var canvas = new DfaCanvas { Document = document, Foreground = Brushes.Black };
        var page = new Border { Background = Brushes.White, Child = canvas };
        page.Measure(Size.Infinity);
        var size = page.DesiredSize;
        page.Arrange(new Rect(size));

        var pixels = new PixelSize(Math.Max(1, (int)Math.Ceiling(size.Width)), Math.Max(1, (int)Math.Ceiling(size.Height)));
        using var bitmap = new RenderTargetBitmap(pixels, new Vector(Dpi, Dpi));
        bitmap.Render(page);

        try
        {
            bitmap.Save(path, PngBitmapEncoderOptions.Default);
            return OpResult.Success;
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        {
            return OpResult.Fail($"Could not save the file: {e.Message}");
        }
    }
}
