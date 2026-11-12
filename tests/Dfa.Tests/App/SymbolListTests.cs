namespace Dfa.Tests.App;

using Dfa.App.Controls;
using Shouldly;

public class SymbolListTests
{
    [Fact]
    public void Parse_SplitsOnCommas_AndTrims()
    {
        SymbolList.Parse(" a, b ,,C ").ShouldBe(["a", "b", "C"]);
    }

    [Fact]
    public void Parse_Empty_IsNoSymbols()
    {
        SymbolList.Parse("  ").ShouldBeEmpty();
    }

    [Fact]
    public void Format_IsTheArrowLabel()
    {
        SymbolList.Format(["a", "b"]).ShouldBe("a, b");
    }
}
