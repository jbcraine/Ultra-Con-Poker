using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Base template for Poker AI

//In inherited classes, use the Awake method to instantiate all of the field values
public abstract class AbstractPokerAI : MonoBehaviour
{
    //How likely is this character to get caught in a betting competition?
    [SerializeField] protected float competitiveness {get; set;}
    //How likely is this character to raise?
    [SerializeField] protected float impetuousness {get; set;}
    //How likely is this character to call large bets?
    [SerializeField] protected float confidence {get; set;}
    //How likely is this character to bluff?
    [SerializeField] protected float bluffiness {get; set;}
    protected AIContestant _contestant;

    protected abstract float HandStrength (List<Card> hand);

    //Once the hand strength is obtained, the character's personality can be factored into the result as to the decision that they will make 
    protected abstract float CharacterInfluence (float handStrength);

    //Use the modifed hand strength to make a decision for this character
    public abstract IPokerCommand MakeDecision ();
}
