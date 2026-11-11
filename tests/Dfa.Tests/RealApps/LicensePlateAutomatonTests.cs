namespace Dfa.Tests.RealApps;

using Dfa.Core.Documents;
using Dfa.Core.Examples;
using Dfa.Core.Simulation;
using Shouldly;

public class LicensePlateAutomatonTests
{
    // The 18 test words of automaton A1, kept in sync with the file's own "tests".
    public static TheoryData<string, bool> Words => new()
    {
        { "B 12 ABC", true },
        { "B 123 ABC", true },
        { "B 001 ZZZ", true },
        { "CJ 12 ABC", true },
        { "AB 01 XYZ", true },
        { "IF 99 AAA", true },
        { "TM 07 BOB", true },
        { "VN 10 QWE", true },
        { "", false },
        { "CJ 123 ABC", false },
        { "B 1 ABC", false },
        { "B 1234 ABC", false },
        { "CJ12ABC", false },
        { "CJ 12 AB", false },
        { "CJ 12 ABCD", false },
        { "CJ 1A ABC", false },
        { "ABC 12 XYZ", false },
        { "cj 12 abc", false },
    };

    private static readonly DfaDocument _plate = ExampleCatalog.Load("license-plate");

    [Theory]
    [MemberData(nameof(Words))]
    public void Word_HasTheExpectedVerdict(string word, bool expected)
    {
        _plate.Dfa.Accepts(word).ShouldBe(expected);
    }

    [Fact]
    public void File_HasTheSameTestWords()
    {
        var expected = Words.Select(row => new TestCase((string)row[0], (bool)row[1]));

        _plate.Tests.ShouldBe(expected);
    }

    [Fact]
    public void Automaton_HasFourteenStatesAndOneFinal()
    {
        _plate.Dfa.States.Count.ShouldBe(14);
        _plate.Dfa.StartState.ShouldBe("q0");
        _plate.Dfa.FinalStates.ShouldBe(["T3"], ignoreOrder: true);
    }

    [Theory]
    [InlineData("", "Enter the county code")]
    [InlineData("C", "Waiting for a second letter, or a space for Bucharest")]
    [InlineData("CJ 1", "Waiting for 1 more digit")]
    [InlineData("B 12", "Waiting for a third digit or a space")]
    [InlineData("CJ 12 ", "Waiting for 3 letters")]
    [InlineData("CJ 12 AB", "Waiting for 1 more letter")]
    [InlineData("CJ 12 ABC", "Valid plate")]
    public void Prefix_EndsInTheStateWhoseDescriptionIsTheMessage(string prefix, string message)
    {
        var run = new DfaRun(_plate.Dfa, prefix);
        run.RunToEnd();

        _plate.Descriptions[run.CurrentState!].ShouldBe(message);
    }

    [Fact]
    public void ThreeDigitsAfterACountyCode_BlockOnTheThirdDigit()
    {
        var run = new DfaRun(_plate.Dfa, "CJ 123 ABC");
        run.RunToEnd();

        run.Reason.ShouldBe(RejectReason.NoTransition);
        run.CurrentState.ShouldBe("J2");
        run.Explanation.ShouldBe("REJECTED: from J2 there is no transition on '3' (class C, position 6).");
    }

    [Fact]
    public void IncrementalRun_MatchesAFullRun()
    {
        var run = new DfaRun(_plate.Dfa, "");

        foreach (var c in "B 123 ABC")
        {
            run.Append(c);
        }

        run.Verdict.ShouldBe(Verdict.Accepted);
        run.RemoveLast();
        run.CurrentState.ShouldBe("T2");
    }
}
