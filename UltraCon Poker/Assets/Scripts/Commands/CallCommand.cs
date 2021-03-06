using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallCommand : IPokerCommand
{
    private AbstractContestant _contestant;

    public CallCommand(AbstractContestant contestant)
    {
        _contestant = contestant;
    }
    public void Execute()
    {
        _contestant.Call();
    }
}
