namespace Dfa.App.Views.TransitionTable;

using CommunityToolkit.Mvvm.Input;

// One entry in a δ cell's menu: a state, or "—" to remove the transition. IsCurrent shows as a check mark.
public sealed record TransitionChoice(string Label, bool IsCurrent, IRelayCommand Pick);
