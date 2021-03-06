using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoldCommand : IPokerCommand
{
    private AbstractContestant _contestant;

    public FoldCommand(AbstractContestant contestant)
    {
        _contestant = contestant;
    }

    public void Execute()
    {
        _contestant.Fold();
    }


}
