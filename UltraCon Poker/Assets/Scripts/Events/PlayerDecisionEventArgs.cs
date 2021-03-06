using System;
public class PlayerDecisionEventArgs : EventArgs
{
    public IPokerCommand playerDecision {get; private set;}

    public PlayerDecisionEventArgs(IPokerCommand decision)
    {
        playerDecision = decision;
    }
}
