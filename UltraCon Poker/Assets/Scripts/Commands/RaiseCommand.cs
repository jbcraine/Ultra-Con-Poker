using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiseCommand : IPokerCommand
{
    private int _raisedBet;
    private AbstractContestant _contestant;

    public RaiseCommand(AbstractContestant contestant, int raisedBet)
    {
        _contestant = contestant;
        _raisedBet = raisedBet;
    }

    public void Execute()
    {
        _contestant.Raise(_raisedBet);
    }
}
