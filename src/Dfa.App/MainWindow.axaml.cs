namespace Dfa.App;

using Avalonia.Controls;
using Dfa.App.Services;

public partial class MainWindow : Window
{
    private bool _closeConfirmed;

    public MainWindow()
    {
        InitializeComponent();
        var theme = new ThemeService(new JsonSettingsStore(JsonSettingsStore.DefaultPath));
        DataContext = new MainViewModel(new DialogService(this), theme);
    }

    // Code-behind exception (constitution III): Closing is a window event with no command to bind, and it cannot
    // await, so it is cancelled, the ViewModel asks "Save changes?", and the window closes again if allowed.
    protected override async void OnClosing(WindowClosingEventArgs e)
    {
        base.OnClosing(e);
        if (_closeConfirmed || DataContext is not MainViewModel main || !main.Session.IsDirty)
        {
            return;
        }

        e.Cancel = true;
        if (await main.ConfirmDiscardAsync())
        {
            _closeConfirmed = true;
            Close();
        }
    }
}
