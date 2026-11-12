namespace Dfa.Tests.RealApps;

using Dfa.App.RealApps.RoPlate;
using Dfa.App.Services;
using Dfa.Core.Documents;
using Shouldly;

public class RoPlateViewModelTests
{
    private readonly FakeNavigation _navigation = new();
    private readonly RoPlateViewModel _plate;

    public RoPlateViewModelTests() => _plate = new RoPlateViewModel(_navigation);

    [Fact]
    public void Empty_ShowsTheStartStatesDescription()
    {
        _plate.Message.ShouldBe("Enter the county code");
        _plate.IsBlocked.ShouldBeFalse();
        _plate.IsValid.ShouldBeFalse();
    }

    [Fact]
    public void TypingOneCharacterAtATime_LowercaseBecomesUppercase()
    {
        Type("cj 12 ");

        _plate.Text.ShouldBe("CJ 12 ");
        _plate.Message.ShouldBe("Incomplete format: waiting for 3 letters");
    }

    [Fact]
    public void AValidPlate_IsGreen()
    {
        Type("B 123 ABC");

        _plate.IsValid.ShouldBeTrue();
        _plate.IsBlocked.ShouldBeFalse();
        _plate.Message.ShouldBe("Valid plate");
    }

    [Fact]
    public void ABlockedPlate_IsRed_WithTheBlockedStatesDescription()
    {
        Type("CJ 123");

        _plate.IsBlocked.ShouldBeTrue();
        _plate.Message.ShouldBe("Invalid format: waiting for a space");
    }

    [Fact]
    public void Backspace_RemovesTheLastStep()
    {
        Type("CJ 123");

        _plate.Text = "CJ 12";

        _plate.IsBlocked.ShouldBeFalse();
        _plate.Message.ShouldBe("Incomplete format: waiting for a space");
    }

    [Fact]
    public void PastingOrEditingInTheMiddle_RunsTheWholeText()
    {
        Type("CJ 12 ABC");

        _plate.Text = "B 12 ABC";
        _plate.IsValid.ShouldBeTrue();

        _plate.Text = "b 1x2 ABC";
        _plate.IsBlocked.ShouldBeTrue();
    }

    [Fact]
    public void ShowAutomaton_OpensTheAutomatonOnTheCurrentText()
    {
        Type("CJ 1");

        _plate.ShowAutomatonCommand.Execute(null);

        _navigation.Opened.ShouldNotBeNull();
        _navigation.Opened.Value.Document.Name.ShouldBe("License plate (RoPlate)");
        _navigation.Opened.Value.Input.ShouldBe("CJ 1");
    }

    private void Type(string text)
    {
        foreach (var c in text)
        {
            _plate.Text += c;
        }
    }

    private sealed class FakeNavigation : INavigationService
    {
        public (DfaDocument Document, string Input)? Opened { get; private set; }

        public void ShowTab(MainTab tab)
        {
        }

        public void OpenInSimulator(DfaDocument document, string input) => Opened = (document, input);

        public void ShowRoPlate()
        {
        }
    }
}
