namespace Dfa.App.Services;

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

    // shortcut: no RoPlateWindow yet, open it here once P3's R-02 is merged.
    public void ShowRoPlate() => main.StatusMessage = "RoPlate: work in progress.";
}
