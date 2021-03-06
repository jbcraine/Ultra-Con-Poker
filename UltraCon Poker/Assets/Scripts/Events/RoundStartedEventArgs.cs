using System;
public class RoundStartedEventArgs : EventArgs
{
    public int _startingMoney {get; private set;}

    public RoundStartedEventArgs(int startingMoney)
    {
        _startingMoney = startingMoney;
    }
}
