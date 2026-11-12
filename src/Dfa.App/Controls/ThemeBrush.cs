namespace Dfa.App.Controls;

using Avalonia.Controls;
using Avalonia.Media;

// The shared brushes from Themes/Styles.axaml (contract 3, section 5), for controls that paint in code.
public static class ThemeBrush
{
    public static IBrush Find(Control control, string key, IBrush fallback) =>
        control.TryFindResource(key, control.ActualThemeVariant, out var value) && value is IBrush brush ? brush : fallback;
}
