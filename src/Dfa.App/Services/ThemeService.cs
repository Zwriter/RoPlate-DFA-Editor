namespace Dfa.App.Services;

using Avalonia;
using Avalonia.Styling;

// Sets Application.RequestedThemeVariant (every DynamicResource follows) and saves the choice.
public sealed class ThemeService : IThemeService
{
    private readonly ISettingsStore _store;

    public ThemeService(ISettingsStore store)
    {
        _store = store;
        Current = store.Load().Theme;
        SetVariant(Current);
    }

    public AppTheme Current { get; private set; }

    public void Apply(AppTheme theme)
    {
        Current = theme;
        SetVariant(theme);
        _store.Save(_store.Load() with { Theme = theme });
    }

    private static void SetVariant(AppTheme theme)
    {
        if (Application.Current is { } app)
        {
            app.RequestedThemeVariant = theme switch
            {
                AppTheme.Light => ThemeVariant.Light,
                AppTheme.Dark => ThemeVariant.Dark,
                _ => ThemeVariant.Default,
            };
        }
    }
}
