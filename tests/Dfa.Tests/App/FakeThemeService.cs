namespace Dfa.Tests.App;

using Dfa.App.Services;

// Records the themes applied by the ViewModels, without touching Application.
internal sealed class FakeThemeService : IThemeService
{
    public FakeThemeService(AppTheme initial = AppTheme.System) => Current = initial;

    public AppTheme Current { get; private set; }

    public int Applies { get; private set; }

    public void Apply(AppTheme theme)
    {
        Current = theme;
        Applies++;
    }
}
