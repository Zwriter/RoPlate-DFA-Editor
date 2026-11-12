namespace Dfa.App.Views.Settings;

using CommunityToolkit.Mvvm.ComponentModel;
using Dfa.App.Services;

// Settings ▸ Theme: three radio menu items (System / Light / Dark) bound two-way to these flags.
public sealed class SettingsViewModel : ObservableObject
{
    private readonly IThemeService _theme;

    public SettingsViewModel(IThemeService theme) => _theme = theme;

    public bool IsSystemTheme
    {
        get => _theme.Current == AppTheme.System;
        set => Select(AppTheme.System, value);
    }

    public bool IsLightTheme
    {
        get => _theme.Current == AppTheme.Light;
        set => Select(AppTheme.Light, value);
    }

    public bool IsDarkTheme
    {
        get => _theme.Current == AppTheme.Dark;
        set => Select(AppTheme.Dark, value);
    }

    // Only the item being checked acts; the radio group unchecking the others is ignored.
    private void Select(AppTheme theme, bool isChecked)
    {
        if (!isChecked || _theme.Current == theme)
        {
            return;
        }

        _theme.Apply(theme);
        OnPropertyChanged(nameof(IsSystemTheme));
        OnPropertyChanged(nameof(IsLightTheme));
        OnPropertyChanged(nameof(IsDarkTheme));
    }
}
