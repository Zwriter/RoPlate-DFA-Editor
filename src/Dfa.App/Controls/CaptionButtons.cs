namespace Dfa.App.Controls;

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;

// The window's minimize / maximize-restore / close buttons, drawn in the app's menu row because the windows use
// WindowDecorations="BorderOnly" (no system title bar). Window mechanics only; the look is its ControlTheme in
// Themes/Controls.axaml.
[TemplatePart("PART_Minimize", typeof(Button))]
[TemplatePart("PART_Maximize", typeof(Button))]
[TemplatePart("PART_Close", typeof(Button))]
[PseudoClasses(":maximized")]
public sealed class CaptionButtons : TemplatedControl
{
    public static readonly StyledProperty<bool> CanMinimizeProperty =
        AvaloniaProperty.Register<CaptionButtons, bool>(nameof(CanMinimize), true);

    public static readonly StyledProperty<bool> CanMaximizeProperty =
        AvaloniaProperty.Register<CaptionButtons, bool>(nameof(CanMaximize), true);

    private Window? _window;

    public bool CanMinimize
    {
        get => GetValue(CanMinimizeProperty);
        set => SetValue(CanMinimizeProperty, value);
    }

    public bool CanMaximize
    {
        get => GetValue(CanMaximizeProperty);
        set => SetValue(CanMaximizeProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        Hook(e, "PART_Minimize", () => _window!.WindowState = WindowState.Minimized);
        Hook(e, "PART_Maximize", ToggleMaximized);
        Hook(e, "PART_Close", () => _window!.Close());
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _window = TopLevel.GetTopLevel(this) as Window;
        if (_window is not null)
        {
            _window.PropertyChanged += OnWindowPropertyChanged;
            UpdateMaximized();
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        if (_window is not null)
        {
            _window.PropertyChanged -= OnWindowPropertyChanged;
            _window = null;
        }
    }

    private void Hook(TemplateAppliedEventArgs e, string name, Action action)
    {
        if (e.NameScope.Find<Button>(name) is { } button)
        {
            button.Click += (_, args) =>
            {
                if (_window is not null)
                {
                    action();
                }

                args.Handled = true;
            };
        }
    }

    private void ToggleMaximized() =>
        _window!.WindowState = _window.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

    private void OnWindowPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == Window.WindowStateProperty)
        {
            UpdateMaximized();
        }
    }

    private void UpdateMaximized() => PseudoClasses.Set(":maximized", _window?.WindowState == WindowState.Maximized);
}
