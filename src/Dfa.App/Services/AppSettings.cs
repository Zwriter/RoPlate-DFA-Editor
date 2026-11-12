namespace Dfa.App.Services;

// What the app remembers between runs (stored by ISettingsStore).
public sealed record AppSettings(AppTheme Theme = AppTheme.System);
