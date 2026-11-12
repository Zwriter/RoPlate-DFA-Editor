namespace Dfa.Tests.App;

using Dfa.App.Services;
using Shouldly;

public sealed class JsonSettingsStoreTests : IDisposable
{
    private readonly string _dir = Directory.CreateTempSubdirectory("roplate-settings-").FullName;

    public void Dispose() => Directory.Delete(_dir, recursive: true);

    [Fact]
    public void A_missing_file_gives_the_defaults()
    {
        var store = new JsonSettingsStore(Path.Combine(_dir, "none", "settings.json"));

        store.Load().ShouldBe(new AppSettings(AppTheme.System));
    }

    [Fact]
    public void Saved_settings_load_back_and_the_folder_is_created()
    {
        var path = Path.Combine(_dir, "nested", "settings.json");
        var store = new JsonSettingsStore(path);

        store.Save(new AppSettings(AppTheme.Dark));

        File.ReadAllText(path).ShouldContain("\"Dark\"");
        new JsonSettingsStore(path).Load().Theme.ShouldBe(AppTheme.Dark);
    }

    [Theory]
    [InlineData("{ not json")]
    [InlineData("{\"Theme\": \"Purple\"}")]
    public void A_malformed_file_gives_the_defaults_without_throwing(string content)
    {
        var path = Path.Combine(_dir, "settings.json");
        File.WriteAllText(path, content);

        new JsonSettingsStore(path).Load().ShouldBe(new AppSettings());
    }
}
