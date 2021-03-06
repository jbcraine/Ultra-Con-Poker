using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Hand))]
public class AIContestant : AbstractContestant
{
    [SerializeField] private AbstractPokerAI _character;    

    private void Awake() {
        _hand = GetComponent<Hand>();
        _character = GetComponent<AbstractPokerAI>();
    }
    public override IPokerCommand MakeDecision ()
    {
        return _character.MakeDecision();
    }

}
