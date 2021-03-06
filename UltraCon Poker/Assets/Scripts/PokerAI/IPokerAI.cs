using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Base template for Poker AI

public interface IPokerAI
{
    //How likely is this character to get caught in a betting competition?
    float competitiveness {get; set;}
    //How likely is this character to take risks on bad hands?
    float impetuousness {get; set;}
    //How likely is this character to bluff?
    float confidence {get; set;}
    //How likely is this character to call large bets?
    float bluffiness {get; set;}

    
    float HandStrength (List<Card> hand);

    //Once the hand strength is obtained, the character's personality can be factored into the result as to the decision that they will make 
    float CharacterInfluence (float handStrength);

    //Use the modifed hand strength to make a decision for this character
    IPokerCommand MakeDecision (Hand hand, int moneyAvaialbe, PokerManager game);
}
