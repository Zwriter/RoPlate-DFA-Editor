namespace Dfa.App;

using Avalonia.Controls;
using Dfa.App.Services;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var theme = new ThemeService(new JsonSettingsStore(JsonSettingsStore.DefaultPath));
        DataContext = new MainViewModel(new DialogService(this), theme);
    }
}
