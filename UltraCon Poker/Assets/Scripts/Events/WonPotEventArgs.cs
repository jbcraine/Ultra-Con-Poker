using System;

public class WonPotEventArgs : EventArgs
{
    public string winnerName {get; private set;}
    public string handName {get; private set;}
    public bool showDownHappened {get; private set;}
    public bool showDownDrawed {get; private set;}

    public WonPotEventArgs(string winnerName, string handName, bool showDownHappened, bool showDownDrawed)
    {
        this.winnerName = winnerName;
        this.handName = handName;
        this.showDownHappened = showDownHappened;
        this.showDownDrawed = showDownDrawed;
    }
}
