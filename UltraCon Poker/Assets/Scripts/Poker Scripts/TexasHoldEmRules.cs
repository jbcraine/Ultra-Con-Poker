using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TexasHoldEmRules : MonoBehaviour, IPokerGameType
{
    public int cardsPerPlayer {get; private set;} = 2;
    public int numCommunityCards {get; private set;} = 5;
    //Every cycle, reveal this many community cards
    public int communityCardRevealedPerPhase {get; private set;} = 1;
    public PhaseDirection direction {get; private set;} = PhaseDirection.Clockwise;
    //The first contestant to bet is the first in the lists
    public int startingContestant {get; private set;} = 0;
    //Two players have blinds
    public BlindsType blinds {get; private set;} = BlindsType.Two;
    public int bigBlinds {get; private set;} = 400;
    public int smallBlinds {get; private set;} = 200;
    public int roundsToRaiseBlinds {get; private set;} = 5;
    public int startingMoney {get; private set;} = 10000;
    public int maxPhases {get; private set;} = 4;
    public HandEvaluator evaluator {get; private set;}

    public (double, string, int[]) ScoreHand(Hand hand, List<Card> communityPool = null)
    {
        return (0, null, null);
    }

    public List<AbstractContestant> DetermineWinner(List<AbstractContestant> contestants)
    {
        return null;
    }

    public List<AbstractContestant> Tiebreaker(List<AbstractContestant> tiedContestants)
    {
        return null;
    }

    public int NextContestant(int contestant, List<AbstractContestant> contestantsInRound)
    {
        if (++contestant > contestantsInRound.Count - 1)
            return 0;
        return contestant;
    }

    //Apply the blinds to the affected contestants, and return the actual amount received from contestants
    public void ApplyBlinds(List<AbstractContestant> contestants, int startingContestantIndex, int contestantsRemainingInRound)
    {
    }

    public void RaiseBlinds(int roundNumber)
    {
        if (roundNumber <= 5)
        {
            smallBlinds = 100;
            bigBlinds = 200;     
        }
        else if (roundNumber <= 10)
        {
            smallBlinds = 200;
            bigBlinds = 400;
        }
        else if (roundNumber<= 15)
        {
            smallBlinds = 500;
            bigBlinds = 1000;
        }
        else if (roundNumber <= 20)
        {
            smallBlinds = 1000;
            bigBlinds = 2500;
        }
        else if (roundNumber > 25)
        {
            smallBlinds = 2500;
            bigBlinds = 5000;
        }
    }

    public void NextPhase()
    {
        //Depending on the phase, reveal three cards, one, or have a showdown
    }
}
