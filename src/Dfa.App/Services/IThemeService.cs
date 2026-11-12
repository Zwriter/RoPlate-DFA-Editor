namespace Dfa.App.Services;

// Applies and remembers the color theme (Settings ▸ Theme).
public interface IThemeService
{
    AppTheme Current { get; }

    void Apply(AppTheme theme);
}
