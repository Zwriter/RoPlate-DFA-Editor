namespace Dfa.Tests.App;

using Dfa.App.Services;
using Dfa.App.Views.Simulator;
using Dfa.Core.Documents;
using Dfa.Core.Examples;
using Shouldly;

public class BatchTestViewModelTests
{
    private readonly DocumentSession _session = new();
    private readonly BatchTestViewModel _tests;

    public BatchTestViewModelTests()
    {
        _session.Replace(ExampleCatalog.Load("even-a"), null);
        _tests = new BatchTestViewModel(_session);
    }

    [Fact]
    public void Rows_AreTheDocumentsTests()
    {
        _tests.Rows.Select(r => r.ToTestCase()).ShouldBe(_session.Document.Tests);
        _tests.Rows.ShouldAllBe(r => r.Passed == null);
        _tests.Summary.ShouldBeNull();
    }

    [Fact]
    public void RunTests_MarksEveryRow_AndShowsTheSummary()
    {
        _tests.Rows[0].Expected = !_tests.Rows[0].Expected;

        _tests.RunTestsCommand.Execute(null);

        _tests.Rows[0].IsFailed.ShouldBeTrue();
        _tests.Rows.Skip(1).ShouldAllBe(r => r.IsPassed);
        _tests.Summary.ShouldBe($"{_tests.Rows.Count - 1} of {_tests.Rows.Count} correct");
    }

    [Fact]
    public void EditingARow_SavesTheListInTheDocument_AndClearsTheResults()
    {
        _tests.RunTestsCommand.Execute(null);
        var row = _tests.Rows[0];

        row.Word = "aab";

        _session.Document.Tests[0].Word.ShouldBe("aab");
        _tests.Rows[0].ShouldBeSameAs(row);
        row.Passed.ShouldBeNull();
        _tests.Summary.ShouldBeNull();
    }

    [Fact]
    public void AddAndRemove_ChangeTheDocumentsTests()
    {
        var count = _session.Document.Tests.Count;

        _tests.AddCommand.Execute(null);
        _session.Document.Tests.Count.ShouldBe(count + 1);
        _session.Document.Tests[^1].ShouldBe(new TestCase("", false));

        _tests.RemoveCommand.Execute(_tests.Rows[0]);
        _session.Document.Tests.Count.ShouldBe(count);
    }

    [Fact]
    public void AStructuralEdit_ClearsTheResults()
    {
        _tests.RunTestsCommand.Execute(null);

        _session.Document.SetFinal("q1", true);

        _tests.Rows.ShouldAllBe(r => r.Passed == null);
        _tests.Summary.ShouldBeNull();
    }

    [Fact]
    public void ReplacingTheDocument_LoadsItsTests()
    {
        _session.Replace(ExampleCatalog.Load("div3"), null);

        _tests.Rows.Select(r => r.ToTestCase()).ShouldBe(_session.Document.Tests);
    }

    [Fact]
    public void OpenInSimulator_RaisesTheRowsWord()
    {
        string? opened = null;
        _tests.OpenInSimulatorRequested += (_, word) => opened = word;

        _tests.OpenInSimulatorCommand.Execute(_tests.Rows[1]);

        opened.ShouldBe(_tests.Rows[1].Word);
    }
}
