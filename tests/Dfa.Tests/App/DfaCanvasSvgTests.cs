namespace Dfa.Tests.App;

using System.Xml.Linq;
using Dfa.App.Controls;
using Dfa.Core.Documents;
using Shouldly;

public class DfaCanvasSvgTests
{
    private static readonly XNamespace _svg = "http://www.w3.org/2000/svg";

    private static DfaDocument TwoStates()
    {
        var doc = DfaDocument.CreateEmpty();
        doc.AddState("q0", new Point2(100, 100));
        doc.AddState("q1", new Point2(300, 100));
        doc.AddSymbol("a");
        doc.AddSymbol("b");
        doc.AddTransitions("q0", ["a", "b"], "q1");
        doc.AddTransitions("q1", ["a"], "q1");
        doc.SetStart("q0");
        doc.SetFinal("q1", true);
        doc.SetDescription("q1", "seen <at least> one symbol & more");
        return doc;
    }

    [Fact]
    public void ToSvg_DrawsStatesFinalsArrowsAndLabels()
    {
        var root = DfaCanvasSvg.ToSvg(TwoStates()).Root!;

        root.Elements(_svg + "circle").Count().ShouldBe(3); // q0, q1, q1's inner circle
        root.Elements(_svg + "path").Count().ShouldBe(3); // q0 → q1, the loop, the start arrow
        root.Elements(_svg + "polygon").Count().ShouldBe(3);
        root.Elements(_svg + "text").Select(t => t.HasElements ? string.Join(" ", t.Elements().Select(l => l.Value)) : t.Value)
            .ShouldBe(["a, b", "a", "q0", "q1", "seen <at least> one symbol & more", "start"], ignoreOrder: true);
    }

    [Fact]
    public void ToSvg_HasTheCanvasSize_AndEscapesText()
    {
        var doc = TwoStates();
        var size = DfaCanvas.DrawingSize(doc);

        var xml = DfaCanvasSvg.ToSvg(doc).ToString();

        XDocument.Parse(xml).Root!.Attribute("width")!.Value.ShouldBe(size.Width.ToString(System.Globalization.CultureInfo.InvariantCulture));
        xml.ShouldContain("&lt;at least&gt;");
        xml.ShouldContain("&amp;");
    }

    [Fact]
    public void Wrap_BreaksAtSpaces_AndKeepsLongWordsWhole()
    {
        DfaCanvasSvg.Wrap("Incomplete format: waiting for 3 letters", 24)
            .ShouldBe(["Incomplete format:", "waiting for 3 letters"]);
        DfaCanvasSvg.Wrap("abcdefghijklmnopqrstuvwxyz z", 10).ShouldBe(["abcdefghijklmnopqrstuvwxyz", "z"]);
        DfaCanvasSvg.Wrap("   ", 10).ShouldBeEmpty();
    }

    [Fact]
    public void SaveSvg_ToAMissingFolder_FailsWithAMessage()
    {
        var result = DfaCanvasSvg.SaveSvg(TwoStates(), Path.Combine(Path.GetTempPath(), "no-such-dir-" + Guid.NewGuid(), "a.svg"));

        result.Ok.ShouldBeFalse();
        result.Error!.ShouldStartWith("Could not save the file:");
    }
}
