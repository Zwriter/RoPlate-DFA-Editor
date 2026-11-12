namespace Dfa.Tests.RealApps;

using Dfa.Core.Documents;
using Dfa.Core.Examples;
using Dfa.Core.Simulation;
using Dfa.Core.Validation;
using Shouldly;

// R-03: the variant with transitions only on the real county codes.
public class LicensePlateCountiesTests
{
    private static readonly string[] _counties =
    [
        "AB", "AG", "AR", "BC", "BH", "BN", "BR", "BT", "BV", "BZ", "CJ", "CL", "CS", "CT", "CV", "DB", "DJ",
        "GJ", "GL", "GR", "HD", "HR", "IF", "IL", "IS", "MH", "MM", "MS", "NT", "OT", "PH", "SB", "SJ", "SM",
        "SV", "TL", "TM", "TR", "VL", "VN", "VS",
    ];

    private static readonly DfaDocument _plate = ExampleCatalog.Load("license-plate-counties");

    public static TheoryData<string> Counties => [.. _counties];

    [Theory]
    [MemberData(nameof(Counties))]
    public void EveryCountyCode_IsAccepted(string county)
    {
        _plate.Dfa.Accepts($"{county} 12 ABC").ShouldBeTrue();
    }

    [Fact]
    public void EveryOtherPairOfLetters_IsRejected()
    {
        var letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var others = letters.SelectMany(a => letters.Select(b => $"{a}{b}")).Except(_counties);

        others.Where(pair => _plate.Dfa.Accepts($"{pair} 12 ABC")).ShouldBeEmpty();
    }

    [Theory]
    [InlineData("B 12 ABC", true)]
    [InlineData("B 123 ABC", true)]
    [InlineData("X 12 ABC", false)]
    public void OnlyBStandsAlone(string word, bool expected)
    {
        _plate.Dfa.Accepts(word).ShouldBe(expected);
    }

    [Fact]
    public void File_IsCompleteDescribedAndPassesItsOwnTests()
    {
        DfaValidator.Validate(_plate.Dfa).Where(i => i.Severity != Severity.Info).ShouldBeEmpty();
        _plate.Layout.Keys.ShouldBe(_plate.Dfa.States, ignoreOrder: true);
        _plate.Descriptions.Keys.ShouldBe(_plate.Dfa.States, ignoreOrder: true);
        BatchTester.Run(_plate.Dfa, _plate.Tests).Results.Where(r => !r.Passed).ShouldBeEmpty();
    }
}
