namespace Dfa.Tests.App;

using Dfa.App.Services;
using Dfa.App.Views.Settings;
using Shouldly;

public sealed class SettingsViewModelTests
{
    [Fact]
    public void Flags_reflect_the_current_theme()
    {
        var settings = new SettingsViewModel(new FakeThemeService(AppTheme.Dark));

        settings.IsDarkTheme.ShouldBeTrue();
        settings.IsLightTheme.ShouldBeFalse();
        settings.IsSystemTheme.ShouldBeFalse();
    }

    [Fact]
    public void Checking_an_item_applies_its_theme_and_updates_every_flag()
    {
        var theme = new FakeThemeService();
        var settings = new SettingsViewModel(theme);
        var changed = new List<string?>();
        settings.PropertyChanged += (_, e) => changed.Add(e.PropertyName);

        settings.IsLightTheme = true;

        theme.Current.ShouldBe(AppTheme.Light);
        theme.Applies.ShouldBe(1);
        changed.ShouldBe(
            [nameof(SettingsViewModel.IsSystemTheme), nameof(SettingsViewModel.IsLightTheme), nameof(SettingsViewModel.IsDarkTheme)],
            ignoreOrder: true);
    }

    [Fact]
    public void Unchecking_or_rechecking_the_current_item_does_nothing()
    {
        var theme = new FakeThemeService(AppTheme.Light);
        var settings = new SettingsViewModel(theme);

        settings.IsSystemTheme = false;
        settings.IsLightTheme = true;

        theme.Applies.ShouldBe(0);
        theme.Current.ShouldBe(AppTheme.Light);
    }
}
