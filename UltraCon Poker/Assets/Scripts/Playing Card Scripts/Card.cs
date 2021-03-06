using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class Card
{
    [SerializeField] private CardValue _value;
    [SerializeField] private CardSuit _suit;
    //Is the card visible or not
    public bool revealed {get; private set;} = false;

   
    public Card(CardValue value, CardSuit suit)
    {
        _value = value;
        _suit = suit;
    }
    public CardValue value
    {
        get {return _value;}
    }

    public CardSuit suit
    {
        get {return _suit;}
    }
    
    public void ChangeValue(CardValue value)
    {
        _value = value;
    }   

    //Change the value of the card by nudging it up or down randomly by 1
    public void NudgeValue()
    {
        //Randomly choose to nudge up or down. If 0, then nudge down
        if (UnityEngine.Random.Range(0, 1) == 0)
        {
            //Check for an Ace, in which case loop back to King
            if (_value == CardValue.Ace)
            {
                ChangeValue (CardValue.King);
            }
            //Nudge down to the next lowest value
            else
            {
                ChangeValue (_value - 1);
            }
        }
        else
        {
            //Check for a King, in which case loop back to an Ace
            if (_value == CardValue.King)
            {
                ChangeValue (CardValue.Ace);
            }
            else
            {
                //Nudge up to the nexy highest value
                ChangeValue (_value + 1);
            }
        }
    }

    public void ChangeSuit(CardSuit suit)
    {
        _suit = suit;
    }

    //Select a random suit
    public void RandomSuit()
    {
        _suit = (CardSuit)UnityEngine.Random.Range(0,3);
    }

    //Select a random value
    public void RandomValue()
    {
        _value = (CardValue)UnityEngine.Random.Range(0,12);
    }

    public bool IsRank(CardValue value)
    {
        return _value == value;
    }

    public bool isSuit(CardSuit suit)
    {
        return _suit == suit;
    }   


}
