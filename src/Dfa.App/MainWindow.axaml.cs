namespace Dfa.App;

using Avalonia.Controls;
using Dfa.App.Services;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel(new DialogService(this));
    }
}
