namespace Dfa.Tests.App;

using Dfa.App;
using Dfa.App.Services;
using Dfa.Core.Documents;
using Dfa.Core.Examples;
using Shouldly;

public sealed class MainViewModelTests : IDisposable
{
    private readonly FakeDialogs _dialogs = new();
    private readonly MainViewModel _main;
    private readonly string _dir = Directory.CreateTempSubdirectory("roplate-").FullName;

    public MainViewModelTests() => _main = new MainViewModel(_dialogs);

    public void Dispose() => Directory.Delete(_dir, recursive: true);

    [Fact]
    public void Title_ShowsName_AndStarWhenDirty()
    {
        _main.Title.ShouldBe("New automaton — DFA Simulator");

        _main.Session.Document.AddState(new Point2(0, 0));

        _main.Title.ShouldBe("New automaton* — DFA Simulator");
    }

    [Fact]
    public async Task OpenExample_ReplacesTheDocument()
    {
        var example = ExampleCatalog.All[0];

        await _main.OpenExampleCommand.ExecuteAsync(example);

        _main.Session.Document.Dfa.States.ShouldNotBeEmpty();
        _main.Session.IsDirty.ShouldBeFalse();
        _dialogs.Confirms.ShouldBe(0);
    }

    [Fact]
    public async Task New_WhenDirty_AsksAndKeepsTheDocumentIfTheSaveIsCancelled()
    {
        _main.Session.Document.AddState(new Point2(0, 0));
        _dialogs.ConfirmAnswer = true;

        await _main.NewCommand.ExecuteAsync(null);

        _dialogs.Confirms.ShouldBe(1);
        _main.Session.Document.Dfa.States.ShouldHaveSingleItem();
    }

    [Fact]
    public async Task New_WhenDirty_AndTheUserSaysNo_Discards()
    {
        _main.Session.Document.AddState(new Point2(0, 0));

        await _main.NewCommand.ExecuteAsync(null);

        _main.Session.Document.Dfa.States.ShouldBeEmpty();
    }

    [Fact]
    public async Task SaveAs_ThenOpen_RoundTripsAndClearsDirty()
    {
        _main.Session.Document.AddState(new Point2(0, 0));
        _dialogs.PickedFile = Path.Combine(_dir, "a.dfa.json");

        await _main.SaveAsCommand.ExecuteAsync(null);

        _main.Session.IsDirty.ShouldBeFalse();
        _main.Session.FilePath.ShouldBe(_dialogs.PickedFile);

        await _main.NewCommand.ExecuteAsync(null);
        await _main.OpenCommand.ExecuteAsync(null);

        _main.Session.Document.Dfa.States.ShouldHaveSingleItem();
        _main.Session.FilePath.ShouldBe(_dialogs.PickedFile);
    }

    [Fact]
    public async Task Open_InvalidFile_ShowsAllErrors_AndKeepsTheDocument()
    {
        _dialogs.PickedFile = Path.Combine(_dir, "bad.dfa.json");
        File.WriteAllText(_dialogs.PickedFile, "{ not json");

        await _main.OpenCommand.ExecuteAsync(null);

        _dialogs.Errors.ShouldNotBeEmpty();
        _main.Session.FilePath.ShouldBeNull();
    }

    [Fact]
    public void OpenInSimulator_LoadsACopy_AndShowsTheSimulateTab()
    {
        var doc = DfaDocument.CreateEmpty("copy");
        doc.AddState(new Point2(0, 0));

        _main.Navigation.OpenInSimulator(doc, "ab");
        doc.AddState(new Point2(50, 0));

        _main.Session.Document.Name.ShouldBe("copy");
        _main.Session.Document.Dfa.States.ShouldHaveSingleItem();
        _main.SelectedTabIndex.ShouldBe((int)MainTab.Simulator);
    }
}
