using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Raise500 : AbstractPokerAI
{
    private void Awake() {
        _contestant = GetComponent<AIContestant>();
    }
    protected override float HandStrength (List<Card> hand)
    {
        return 0;
    }

    //Once the hand strength is obtained, the character's personality can be factored into the result as to the decision that they will make 
    protected override float CharacterInfluence (float handStrength)
    {
        return 0;
    }

    //This AI always returns a command to bet all of their money
    public override IPokerCommand MakeDecision ()
    {
        int currentBet = _contestant.game.currentBet;
        return new RaiseCommand(_contestant, currentBet + 200);
    }
}
