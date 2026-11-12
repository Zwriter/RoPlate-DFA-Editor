namespace Dfa.Tests.App;

using Dfa.App.Services;

// Records what the ViewModels ask the dialogs, and answers with the values set by the test.
internal sealed class FakeDialogs : IDialogService
{
    public bool ConfirmAnswer { get; set; }

    public int Confirms { get; private set; }

    public string? PickedFile { get; set; }

    public List<string> Errors { get; } = [];

    public Task ShowErrorAsync(string message)
    {
        Errors.Add(message);
        return Task.CompletedTask;
    }

    public Task ShowErrorsAsync(string title, IEnumerable<string> messages)
    {
        Errors.AddRange(messages);
        return Task.CompletedTask;
    }

    public Task ShowInfoAsync(string message) => Task.CompletedTask;

    public Task<bool> ConfirmAsync(string message)
    {
        Confirms++;
        return Task.FromResult(ConfirmAnswer);
    }

    public Task<string?> AskTextAsync(string title, string prompt, string initial = "") => Task.FromResult<string?>(null);

    public Task<string?> PickOpenFileAsync() => Task.FromResult(PickedFile);

    // The first test that needs a save picks the file; a dirty, never-saved document in other tests cancels.
    public Task<string?> PickSaveFileAsync(string suggestedName) => Task.FromResult(PickedFile);

    public Task<string?> PickSaveImageAsync(string suggestedName) => Task.FromResult(PickedFile);
}
