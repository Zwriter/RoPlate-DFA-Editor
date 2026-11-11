namespace Dfa.Tests.Core;

using Dfa.Core.Examples;
using Dfa.Core.Simulation;
using Dfa.Core.Validation;
using Shouldly;

public class ExampleCatalogTests
{
    public static TheoryData<string> Ids => [.. ExampleCatalog.All.Select(e => e.Id)];

    [Fact]
    public void All_ListsTheP1ExamplesInContractOrder()
    {
        ExampleCatalog.All.Select(e => e.Id).Take(6)
            .ShouldBe(["even-a", "starts-a", "ends-ab", "contains-aba", "div3", "length-3"]);
    }

    [Theory]
    [MemberData(nameof(Ids))]
    public void Example_LoadsCompleteAndDescribed(string id)
    {
        var doc = ExampleCatalog.Load(id);

        doc.Name.ShouldBe(ExampleCatalog.All.Single(e => e.Id == id).Title);
        // Info issues (missing transitions) are allowed: license-plate is partial on purpose,
        // a blocked run is how RoPlate shows an invalid plate.
        DfaValidator.Validate(doc.Dfa).Where(i => i.Severity != Severity.Info).ShouldBeEmpty();
        doc.Layout.Keys.ShouldBe(doc.Dfa.States, ignoreOrder: true);
        doc.Descriptions.Keys.ShouldBe(doc.Dfa.States, ignoreOrder: true);
        doc.Tests.ShouldNotBeEmpty();
    }

    [Theory]
    [MemberData(nameof(Ids))]
    public void Example_PassesItsOwnTests(string id)
    {
        var doc = ExampleCatalog.Load(id);

        var result = BatchTester.Run(doc.Dfa, doc.Tests);

        result.Results.Where(r => !r.Passed).Select(r => r.Case.Word).ShouldBeEmpty();
    }

    [Fact]
    public void Load_UnknownId_IsAProgrammingError()
    {
        Should.Throw<ArgumentException>(() => ExampleCatalog.Load("nope"));
    }
}
