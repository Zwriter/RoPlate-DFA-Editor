namespace Dfa.App.Views.Simulator;

// The "Guess" mode score (S-08, SI-09): every answer is confirmed or corrected.
public sealed class GuessGame
{
    public int Correct { get; private set; }

    public int Total { get; private set; }

    public string Score => $"Score: {Correct} of {Total}";

    public void Clear() => (Correct, Total) = (0, 0);

    public string Answer(string guess, string actual)
    {
        Total++;
        if (guess == actual)
        {
            Correct++;
            return "Correct!";
        }

        return $"Wrong: the automaton goes to {actual}";
    }
}
