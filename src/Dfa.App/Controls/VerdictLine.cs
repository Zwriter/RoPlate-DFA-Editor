namespace Dfa.App.Controls;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;

// The verdict line (S-04, A1): ✓ / ✗ next to the contract message, in a status box; plain text while there is no
// verdict yet. One control for the simulator and RoPlate; the look is its ControlTheme in Themes/Controls.axaml.
[PseudoClasses(":accepted", ":rejected")]
public sealed class VerdictLine : TemplatedControl
{
    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<VerdictLine, string?>(nameof(Text));

    public static readonly StyledProperty<bool> IsAcceptedProperty =
        AvaloniaProperty.Register<VerdictLine, bool>(nameof(IsAccepted));

    public static readonly StyledProperty<bool> IsRejectedProperty =
        AvaloniaProperty.Register<VerdictLine, bool>(nameof(IsRejected));

    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public bool IsAccepted
    {
        get => GetValue(IsAcceptedProperty);
        set => SetValue(IsAcceptedProperty, value);
    }

    public bool IsRejected
    {
        get => GetValue(IsRejectedProperty);
        set => SetValue(IsRejectedProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsAcceptedProperty || change.Property == IsRejectedProperty)
        {
            PseudoClasses.Set(":accepted", IsAccepted);
            PseudoClasses.Set(":rejected", IsRejected);
        }
    }
}
