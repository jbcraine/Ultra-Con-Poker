using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//If Texas Hold'Em, give two cards
//If 5 Card Stid, give five cards
//If a player raises, set allContestantsCalled to 0, and go numContestants times untl each one is called or folded.
public class PokerManager : MonoBehaviour
{
    [SerializeField] private Deck _deck;
    [SerializeField] private List<AbstractContestant> _contestants;
    private List<AbstractContestant> _contestantsInMatch;
    private List<Card> _communityCards;
    private List<AbstractContestant> _contestantsInRound;
    private IPokerGameType _ruleSet;

    private  int _currentBet;
    private int _mainPot;
    private List<SidePot> _sidePots;
    private int _round = 0;
    private int _phase = 0;
    //The contestant whose turn it is
    private AbstractContestant _currentContestant;
    public bool isPlayerTurn = false;
    private bool startingContestantEliminated;
    private int _currentContestantIndex;
    private int _startingContestantIndex;
    public event BetChangedEventHandler BetChanged; 
    public event EndRoundEventHandler RoundEnded;
    public event PotChangedEventHandler PotChanged;
    public event RoundStartedEventHandler RoundStarted;
    public event ContestantEliminatedEventHandler ContestantEliminated;
    public event WonPotEventHandler PotWon;
    public event AllContestantsCalledHandler CalledAllContestants;
    public event MatchEndEventHandler MatchEnded;


    public int currentBet
    {
        get {return _currentBet;}
    }

    public int contestantsRemainingInRound
    {
        get {return _contestantsInRound.Count;}
    }
    private void Awake() 
    {
        _ruleSet = GetComponent<IPokerGameType>();
        _sidePots = new List<SidePot>();
    }


    public void StartMatch()
    {
        ResetGame();
        
        //Give every contestant the starting amount of money and set the max size of their hands
        foreach (AbstractContestant contestant in _contestantsInMatch)
        {
            contestant.ChangeMoney(_ruleSet.startingMoney);
            contestant.hand.SetMaxCards(_ruleSet.cardsPerPlayer);
        }

        //Select the initial starting contestant, as well as assigning initial blinds if they are available

        _startingContestantIndex = _currentContestantIndex = Random.Range(0, _contestantsInMatch.Count);
        _currentContestant = _contestantsInMatch[_startingContestantIndex];

        StartNextRound();
    }

    public void StartNextRound()
    {
        //Update contestantsInRound, in case any contestants were eliminated
        _contestantsInRound = new List<AbstractContestant>(_contestantsInMatch);

        //TEMOPORARY
        _currentBet = 0;

        //Use events to communicate with the UI. 
        BetChanged(this, new BetEventArgs(_currentBet));
        PotChanged(this, new PotChangedEventArgs(_mainPot));

        RoundStarted(this, new RoundStartedEventArgs(FindObjectOfType<PlayerContestant>().money));

        _phase = 1;
        _round += 1;

        //Reset the bets of each contestant
        foreach(AbstractContestant contestant in _contestantsInRound)
        {
            contestant.status = ContestantStatus.NotCalled;
            contestant.ResetBet();
        }

        _ruleSet.RaiseBlinds(_round);

        //TODO: If blinds are available, then apply call to the contributing contestants
        if (_ruleSet.blinds != BlindsType.None)
        {
            _ruleSet.ApplyBlinds(_contestantsInRound, _startingContestantIndex, this.contestantsRemainingInRound);
            _currentBet = _ruleSet.bigBlinds;
            BetChanged(this, new BetEventArgs(_currentBet));
            PotChanged(this, new PotChangedEventArgs(_mainPot));
        }

        //Fill and shuffle the deck at the start of every round
        _deck.FillStandardDeck();
        _deck.ShuffleDeck();
        
        //Get cards per player from ruleSet and deal
        foreach(AbstractContestant contestant in _contestantsInRound)
        {
            Deal(contestant);   
        }

         //Get numCommunityCards from ruleSet and deal
        if (_ruleSet.numCommunityCards != 0)
        {
            for (int i = 0; i < _ruleSet.numCommunityCards; ++i)
                _communityCards.Add(_deck.PopCard());
        }

        _currentContestant = _contestantsInRound[_startingContestantIndex];
        StartTurn();
    }


    private void Deal(AbstractContestant contestant)
    {
        List<Card> hand = new List<Card>();
        for (int i = 0; i < contestant.hand.maxCards; ++i)
            hand.Add(_deck.PopCard());

        contestant.FillHand(hand);
    }


#region EndRound
    //Round ends when either all but one contestant folds, or a winning hand is determined
    private void EndRound()
    {
        StartCoroutine ("RewardPots");

    }

    //Use an IEnumerator so as to time the revealing of UI elements saying who one, and with what type of hand
    IEnumerator RewardPots()
    {
        //If more than one contestant remains in the final round, then a showdown decides a winner or draw and the pot is distributed
        if (_contestantsInRound.Count > 1)
        {
            //Check for any all-ins, and create side pots accordingly
            CreateSidePots();

            List<AbstractContestant> roundWinners = _ruleSet.DetermineWinner(_contestantsInRound);
            if (roundWinners.Count == 1)
            {
                AbstractContestant winner = roundWinners[0];

                //Send a message to the UI to display a message saying who won and with what kind of hand
                PotWon(this, new WonPotEventArgs(winner.name, winner.handName, true, false));
                yield return new WaitForSeconds(3);
                RewardMainPot(winner: roundWinners[0]);
            }
            else if (roundWinners.Count > 1)
            {
                string handName = roundWinners[0].handName;
                PotWon(this, new WonPotEventArgs("Draw", handName, true, true));
                RewardMainPot(drawedWinners: roundWinners);
            }
            //If there are any sidepots, then award them out appropriately
            if (_sidePots.Count > 0)
                RewardSidePots();
        }
        //If all other contestants folded, then the last remaining contestant wins the round by default
        else if (_contestantsInRound.Count == 1)
        {
            PotWon(this, new WonPotEventArgs(_currentContestant.name, null, false, false));
            RewardMainPot(winner: _currentContestant);
        }

        StartCoroutine("EliminateContestants");
    }

    //Use an IEnumerator so as to time the revealing of UI elements saying who has been eliminated
    IEnumerator EliminateContestants()
    {
        startingContestantEliminated = false;
        //If a contestant has run out of money, then they are eliminated from play
        foreach (AbstractContestant contestant in _contestantsInRound)
        {
            if (contestant.money == 0)
            {
                //If the starting contestant was eliminated, then immediately set the starting contestant to the next contestant, and raise a flag
                if (_contestantsInRound[_startingContestantIndex].Equals(contestant))
                {
                    _startingContestantIndex = _ruleSet.NextContestant(_startingContestantIndex, _contestantsInMatch);
    
                    startingContestantEliminated = true;
                }
                ContestantEliminated(this, new ContestantEliminatedEventArgs(contestant.name));
                yield return new WaitForSeconds(3);
                RemoveContestantFromMatch(contestant);
            }
        }

        ClearRound();
    }

    //Clear the slate to prepare for the next round
    void ClearRound()
    {
        //Clear each contestant's hand at the end of every round.
        //TODO: Make sure contestants that were removed from the match also have their hands cleared
        foreach(AbstractContestant contestant in _contestantsInMatch)
        {
            contestant.hand.Clear();
            contestant.betMoney = 0;
        }

        if (_contestantsInMatch.Count == 1)
        {
            EndMatch();
        }
        else
        {
            //Update the starting contestant before beginning the next round
            if (!startingContestantEliminated)
                _startingContestantIndex = _ruleSet.NextContestant(_startingContestantIndex, _contestantsInMatch);
        }

        RoundEnded(this);
    }
#endregion

    //Use a list, because, in the case of a draw, there can be considered multiple winners
    private void RewardMainPot(AbstractContestant winner = null, List<AbstractContestant> drawedWinners = null)
    {
        if (winner == null && drawedWinners == null)
        {
            Debug.Log("BIG MISTAKE");
        }
        else if (winner != null && drawedWinners != null)
        {
            Debug.Log("BIGGER MISTAKE");
        }
        else if (drawedWinners != null)
        {
            int portion = _mainPot / drawedWinners.Count;
            foreach(AbstractContestant drawedContestant in drawedWinners)
            {
                drawedContestant.ChangeMoney(portion);
            }
        }
        else if (winner != null)
        {
            winner.ChangeMoney(_mainPot);
        }

        _mainPot = 0;
    }

    private void RewardSidePots()
    {
        //Use the remaining contestant's scores to reward all the side pots
        foreach (SidePot pot in _sidePots)
        {
            List<AbstractContestant> winners = _ruleSet.DetermineWinner(pot.contenders);

            if (winners.Count == 1)
            {
                winners[0].ChangeMoney(pot.pot);
            }
            else if (winners.Count > 1)
            {
                int portion = pot.pot / winners.Count;
                foreach(AbstractContestant drawedWinner in winners)
                    drawedWinner.ChangeMoney(portion);
            }
        }

        //Once sidepots are rewarded out, then reset the list of sidepots
        _sidePots.Clear();
    }

    private void EndMatch()
    {
        //When the match is over, then the manager can be destroyed
        AbstractContestant winner = _contestantsInMatch[0];
        MatchEnded(this, new MatchEndEventArgs(winner.name));
        Debug.Log("End game");
    }

    //Remove a contestant from the round when they fold
    public void RemoveContestantFromRound(AbstractContestant contestant)
    {
        _contestantsInRound.Remove(contestant);
    }

    //Remove a contestant from the game when they run out of money
    private void RemoveContestantFromMatch(AbstractContestant contestant)
    {
        contestant.hand.Clear();
        _contestantsInMatch.Remove(contestant);
    }

    //TODO: Add a check to guarantee that the contestant has not bet more money than they actually have
    public void OnRaise(int raisedBet)
    {
        _currentBet = raisedBet;
        ResetCallStatus();

        PotChanged(this, new PotChangedEventArgs(_mainPot));
        
        //Update the current bet inside the UI
        BetChanged(this, new BetEventArgs(raisedBet));

        NextContestant();
    }

    //TODO: If no contestants can meet the raised bet, then return money to the contestant that initiated the raised bet
    public void OnCall()
    {
        PotChanged(this, new PotChangedEventArgs(_mainPot));

        if (AllContestantsCalled())
            NextPhase();
        NextContestant();
    }

    public void OnCheck()
    {
        if (AllContestantsCalled())
            NextPhase();
        NextContestant();
    }

    public void OnFold(AbstractContestant contestant)
    {
        RemoveContestantFromRound(contestant);
        
        if (AllContestantsCalled())
            NextPhase();
        NextContestant();
    }

    public void NextContestant()
    {
        _currentContestantIndex = _ruleSet.NextContestant(_currentContestantIndex, _contestantsInRound);
        _currentContestant = _contestantsInRound[_currentContestantIndex];
        StartTurn();
    }

    public void StartTurn()
    {
        //If all other contestants have folded, then end the round
        if (_contestantsInRound.Count == 1)
        {
            Debug.Log("Round end: Fold");
            EndRound();
        }

        else if (AllContestantsCalled() && _phase != _ruleSet.maxPhases)
        {
            CalledAllContestants(this);
            NextPhase();
        }

        else if (AllContestantsCalled() && _phase == _ruleSet.maxPhases)
        {
            CalledAllContestants(this);
            EndRound();
        }
        
        else if (_currentContestant.money == 0)
        {
            //If the contestant has gone all-in, then automatically set their status to Called and continue to the next contestant
            _currentContestant.status = ContestantStatus.Called;
            NextContestant();
        }

        else
        {
            //If the current contestant is not the player, then ask for a decision
            if (!_currentContestant.isPlayer)
                _currentContestant.MakeDecision().Execute();
            //Otherwise, activate the UI and prompt the player to make a decision
            else
            {
                isPlayerTurn = true;
                FindObjectOfType<UIController>().ActivateUI();
            }
        }
    }

    private void NextPhase()
    {
        _ruleSet.NextPhase();
    }

    private bool AllContestantsCalled()
    {
        foreach (AbstractContestant contestant in _contestantsInRound)
        {
            if (contestant.status == ContestantStatus.NotCalled)
                return false;
        }

        return true;
    }

    public void ResetCallStatus()
    {
        foreach(AbstractContestant contestant in _contestantsInRound)
        {
            contestant.status = ContestantStatus.NotCalled;
        }
    }

    public void AddToPot(int contribution)
    {
        _mainPot += contribution;
    }


#region SidePots
    //CONDITION: The provided list should only have two entries
    //Provide a refund to whichever contestant made the higher bet
    //REFUND = higherBet - lowerBet
    private void DetermineRefund(List<AbstractContestant> contestants)
    {
        AbstractContestant highestBetter = _contestantsInRound[0];
        if (_contestantsInRound[1].betMoney > highestBetter.betMoney)
        {
            highestBetter = _contestantsInRound[1];
            RefundContestant(highestBetter, _contestantsInRound[0].betMoney);
        }
        else if (_contestantsInRound[1].betMoney < highestBetter.betMoney)
        {
            RefundContestant(highestBetter, _contestantsInRound[1].betMoney);
        }
        //Else, if the contstants bet equally, do nothing
        else
        {
            return;
        }
    }

    //If multiple contestants face showdown, but not all could call the current bet, 
    //then refund the highest better the difference of their bet and the next greatest bet
    private void RefundContestant(AbstractContestant highestBetter, int nextHighestBet)
    {
        if (contestantsRemainingInRound < 2)
            return;

        //Determine the amount to be refunded, and adjust the highestBettingContest's money accordingly
        int refund = highestBetter.betMoney - nextHighestBet;
        highestBetter.ChangeMoney(refund);
    }

    //Try to create additional side pots in the event of all-ins
    private void CreateSidePots()
    {
        //Side pots are only created when more than 3 contestants have reached the showdown phase
        //If there are only two contestants, then see if one of them is entitled to a refund
        if (contestantsRemainingInRound < 3)
        {
            DetermineRefund(_contestantsInRound);
            return;
        }
        
        int numAllIn = 0;
        foreach(AbstractContestant contestant in _contestantsInRound)
        {
            if (contestant.allIn)
                ++numAllIn;                
        }

        //Side pots are only created if there exists a discrepency between the amounts of the contestants all-ins
        if (numAllIn < 1)
            return;

        List<AbstractContestant> contestantsByAmountBet = SortContestantsFromBet(_contestantsInRound);

        //The main pot is determined by the lowest bet that all contestants have met. Each contestant can claim this pot
        int mainPot;
        int lowBet = contestantsByAmountBet[0].betMoney;
        mainPot = lowBet * contestantsByAmountBet.Count;
        foreach(AbstractContestant contestant in contestantsByAmountBet)
            {
                if (contestant.betMoney == lowBet)
                    contestantsByAmountBet.Remove(contestant);
            }

        //Create as many side pots as necessary (Generally count - 2, potentially fewer if multiple contestants have identical bets)
        //If only one contestant has the highest call, then that contestant gets a refund
        //If multiple contestants have the highest call, then they take part in the highest side pot
        while (contestantsByAmountBet.Count > 1)
        {
            //Bet is the lowest bet among the contestants
            int bet = contestantsByAmountBet[0].betMoney;
            int remainder = bet - lowBet;
            int sidePotAmount = remainder * (contestantsByAmountBet.Count);
            _sidePots.Add(new SidePot(contestantsByAmountBet, sidePotAmount));

            //Do not consider any contestant who has wagered up to current bet for any future side pots
            foreach(AbstractContestant contestant in contestantsByAmountBet)
            {
                if (contestant.betMoney == bet)
                    contestantsByAmountBet.Remove(contestant);
            }

            //Use the previous bet to determine remainders for the next sidePot
            lowBet = bet;
        }

        //The highest betting contestant is entitled to a refund of however much money no other contestant could match in their bet
        if (contestantsByAmountBet.Count == 1)
        {
            RefundContestant(contestantsByAmountBet[0], lowBet);
        }
    }

    private List<AbstractContestant> SortContestantsFromBet(List<AbstractContestant> contestants)
    {
        if (contestants.Count == 0)
        {
            return contestants;
        }

        //Sort the contestants by the amount they have bet. This will be used to adjust the main pot and create side pots
        //This is done using a basic insertion sort
        List<AbstractContestant> contestantsByAmountBet = new List<AbstractContestant>(_contestantsInRound);
        int n = contestantsByAmountBet.Count;

        for (int i = 1; i < n; ++i)
        {
            AbstractContestant key = contestantsByAmountBet[i];
            int j = i - 1;

            while (j >= 0 && contestantsByAmountBet[j].betMoney > key.betMoney)
            {
                contestantsByAmountBet[j + 1] = contestantsByAmountBet[j];
                j = j - 1;
            }
            contestantsByAmountBet[j + 1] = key;
        }

        return contestantsByAmountBet;
    }
#endregion

    //Reset the game state to the starting conditions
    private void ResetGame()
    {
        _round = 0;
        _phase = 0;
        _mainPot = 0;
        _currentBet = 0;

        _contestantsInMatch = new List<AbstractContestant>(_contestants);

        foreach(AbstractContestant contestant in _contestantsInMatch)
        {
            contestant.money = 0;
            contestant.betMoney = 0;
            contestant.status = ContestantStatus.NotCalled;
            contestant.AssignScore(0, "", null);
            contestant.hand.Clear();
        }
    }

}
