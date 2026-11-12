namespace Dfa.App.Services;

using System.Threading.Tasks;
using Dfa.App.RealApps.RoPlate;
using Dfa.Core.Documents;

public sealed class NavigationService(MainViewModel main, DocumentSession session) : INavigationService
{
    public void ShowTab(MainTab tab) => main.SelectedTabIndex = (int)tab;

    public void OpenInSimulator(DfaDocument document, string input) => _ = OpenInSimulatorAsync(document.Clone(), input);

    public void ShowRoPlate() => new RoPlateWindow { DataContext = new RoPlateViewModel(this) }.Show();

    // Like New / Open: unsaved changes are offered for saving before the document is replaced.
    private async Task OpenInSimulatorAsync(DfaDocument copy, string input)
    {
        if (!await main.ConfirmDiscardAsync())
        {
            return;
        }

        session.Replace(copy, null);
        ShowTab(MainTab.Simulator);
        main.Simulator.Start(input);
    }
}
