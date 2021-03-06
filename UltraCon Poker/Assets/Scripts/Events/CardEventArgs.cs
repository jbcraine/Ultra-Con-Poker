using System;

public class CardEventArgs : EventArgs
{
    public CardValue cardValue { get; private set; }
    public CardSuit cardSuit {get; private set;}

    public CardEventArgs (CardValue value, CardSuit suit)
    {
        cardValue = value;
        cardSuit = suit;
    }
}