namespace Dfa.App.Services;

// Loads and saves AppSettings. Never throws: a missing or unreadable store gives the defaults.
public interface ISettingsStore
{
    AppSettings Load();

    void Save(AppSettings settings);
}
