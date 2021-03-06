using System.Collections.Generic;

public interface IPokerGameType
{
    int cardsPerPlayer {get;}
    int numCommunityCards {get;}
    PhaseDirection direction {get;}
    int startingContestant {get;}
    BlindsType blinds {get;}
    int bigBlinds {get;}
    int smallBlinds {get;}
    int roundsToRaiseBlinds {get;}
    int startingMoney {get;}
    int maxPhases {get;}
    HandEvaluator evaluator {get;}
    
    (double, string, int[]) ScoreHand (Hand hand, List<Card> communityPool = null);

    List<AbstractContestant> DetermineWinner (List<AbstractContestant> contestants);


    int NextContestant (int contestant, List<AbstractContestant> contestantsInMatch);

    void NextPhase();

    void RaiseBlinds(int roundNumber);

    void ApplyBlinds(List<AbstractContestant> contestants, int startingContestantIndex, int contestantsInRound);
}
