namespace Dfa.App.Views.Simulator;

using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Dfa.Core.Documents;

// One editable row of the batch test list (S-07): the word, the "expected accepted" check and, after a run, ✓ / ✗.
public sealed partial class BatchTestRow : ObservableObject
{
    private readonly Action _edited;

    [ObservableProperty]
    private string _word;

    [ObservableProperty]
    private bool _expected;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsPassed), nameof(IsFailed))]
    private bool? _passed;

    public BatchTestRow(TestCase testCase, Action edited)
    {
        _word = testCase.Word;
        _expected = testCase.Expected;
        _edited = edited;
    }

    public bool IsPassed => Passed == true;

    public bool IsFailed => Passed == false;

    public TestCase ToTestCase() => new(Word, Expected);

    partial void OnWordChanged(string value) => _edited();

    partial void OnExpectedChanged(bool value) => _edited();
}
