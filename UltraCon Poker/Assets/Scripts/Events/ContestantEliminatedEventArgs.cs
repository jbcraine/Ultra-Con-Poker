using System;

public class ContestantEliminatedEventArgs : EventArgs
{
    public string contestantName {get; private set;}

    public ContestantEliminatedEventArgs(string contestantName)
    {
        this.contestantName = contestantName;
    }
}
