using System;

public class MatchEndEventArgs : EventArgs
{
    public string winnerName {get; private set;}

    public MatchEndEventArgs(string winnerName)
    {
        this.winnerName = winnerName;
    }    

    
}
