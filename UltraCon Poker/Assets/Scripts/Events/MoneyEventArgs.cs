using System;

public class MoneyEventArgs : EventArgs
{
    public int money {get; private set;}

    public MoneyEventArgs (int money)
    {
        this.money = money;
    }
}