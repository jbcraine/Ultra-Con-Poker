using System;

public class BetEventArgs : EventArgs
{
    public int currentBet {get; private set;}

    public BetEventArgs(int currentBet)
    {
        this.currentBet = currentBet;
    }
}
