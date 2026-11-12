namespace Dfa.App.Services;

using Dfa.App.RealApps.RoPlate;
using Dfa.Core.Documents;

public sealed class NavigationService(MainViewModel main, DocumentSession session) : INavigationService
{
    public void ShowTab(MainTab tab) => main.SelectedTabIndex = (int)tab;

    public void OpenInSimulator(DfaDocument document, string input)
    {
        session.Replace(document.Clone(), null);
        ShowTab(MainTab.Simulator);
        main.Simulator.Start(input);
    }

    public void ShowRoPlate() => new RoPlateWindow { DataContext = new RoPlateViewModel(this) }.Show();
}
