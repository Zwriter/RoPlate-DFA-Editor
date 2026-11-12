namespace Dfa.Tests.App;

using Dfa.App.Services;
using Dfa.App.Views.Editor;
using Dfa.Core.Examples;
using Dfa.Core.Model;
using Shouldly;

public class EditorViewModelTests
{
    private readonly DocumentSession _session = new();
    private readonly EditorViewModel _editor;

    public EditorViewModelTests() => _editor = new EditorViewModel(_session, new FakeDialogs());

    [Fact]
    public void AddSymbol_AddsToSigma_AndClearsTheField()
    {
        _editor.NewSymbol = "a";

        _editor.AddSymbolCommand.Execute(null);

        _session.Document.Dfa.Alphabet.ShouldBe(["a"]);
        _editor.Symbols.ShouldBe(["a"]);
        _editor.NewSymbol.ShouldBe("");
        _editor.SymbolError.ShouldBeNull();
    }

    [Fact]
    public void AddSymbol_Refused_ShowsTheMessageBelowTheField()
    {
        _editor.NewSymbol = "a";
        _editor.AddSymbolCommand.Execute(null);
        _editor.NewSymbol = "a";

        _editor.AddSymbolCommand.Execute(null);

        _editor.SymbolError.ShouldBe("Symbol 'a' is already in the alphabet.");
        _editor.NewSymbol.ShouldBe("a");
    }

    [Fact]
    public void AddSymbol_IsDisabledWhenEmpty()
    {
        _editor.AddSymbolCommand.CanExecute(null).ShouldBeFalse();
        _editor.AddClassCommand.CanExecute(null).ShouldBeFalse();
    }

    [Fact]
    public void RemoveSymbol_UsedInTransitions_ShowsTheMessage()
    {
        _session.Replace(ExampleCatalog.Load("even-a"), null);

        _editor.RemoveSymbolCommand.Execute("a");

        _editor.SymbolError.ShouldBe("Symbol 'a' is used in 2 transitions. Remove them first.");
        _editor.Symbols.ShouldBe(["a", "b"]);
    }

    [Fact]
    public void AddClass_UsesRanges()
    {
        _editor.NewClassName = "C";
        _editor.NewClassCharacters = "0-9";

        _editor.AddClassCommand.Execute(null);

        _editor.Classes.ShouldBe([new SymbolClass("C", "0123456789")]);
        _editor.Symbols.ShouldBeEmpty();
        _editor.NewClassName.ShouldBe("");
        _editor.ClassError.ShouldBeNull();
    }

    [Fact]
    public void AddClass_Overlapping_ShowsTheMessage()
    {
        _editor.NewSymbol = "5";
        _editor.AddSymbolCommand.Execute(null);
        _editor.NewClassName = "C";
        _editor.NewClassCharacters = "0-9";

        _editor.AddClassCommand.Execute(null);

        _editor.ClassError.ShouldBe("Symbol '5' is already in the alphabet.");
    }

    [Fact]
    public void RemoveClass_UsedInTransitions_ShowsTheMessage()
    {
        _session.Replace(ExampleCatalog.Load("license-plate"), null);

        _editor.Classes.Count.ShouldBe(3);
        _editor.RemoveClassCommand.Execute(_editor.Classes[0]);

        _editor.ClassError.ShouldBe("Symbol 'L' is used in 5 transitions. Remove them first.");
    }
}
