using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class AbstractContestant : MonoBehaviour
{
    [SerializeField] protected string _contestantName;
    protected Hand _hand;
    [SerializeField] protected List<PokerManager> _games;
    //The money that the contestant has left available to wager in a round
    protected int _money;
    //The money that the contestant has already wagered in a round
    protected int _betMoney;
    protected ContestantStatus _status;
    protected bool _isPlayer = false;
    protected bool _allIn = false;
    protected double _handScore = 0;
    protected string _handName;
    protected int[] _bestPossibleHand;

    public int money
    {
        get {return _money;}
        set {_money = value;}
    }

    public int betMoney
    {
        get {return _betMoney;}
        set {_betMoney = value;}
    }

    public Hand hand
    {
        get {return _hand;}
    }

    public ContestantStatus status
    {
        get {return _status;}
        set {_status = value;}
    }

    public bool isPlayer
    {
        get {return _isPlayer;}
    }

    public bool allIn
    {
        get {return _allIn;}
    }

    public PokerManager game
    {
        get {return _games[0];}
    }

    public double score
    {
        get {return _handScore;}
    }

    public string handName
    {
        get {return _handName;}
    }

    public string contestantName
    {
        get {return contestantName;}
    }

    public void FillHand(List<Card> cards)
    {
        foreach (Card card in cards)
        {
            //Debug.Log(card.value + " of " + card.suit);
            _hand.AddCard(card);
        }
    }

    //Get the current bet from the manager, and find the difference
    public void Raise(int raisedBet)
    {
        //Make sure the contestant cannot bet more money than then have
        if (raisedBet >= _betMoney + money)
        {
            raisedBet = _betMoney + money;
        }

        int contribution = raisedBet - _betMoney;

        //Make sure that the raisedBet is actually higher than the currentBet in the game
        //Make sure that the contestant cannot bet more monet rhan they actually have
        if (raisedBet > _games[0].currentBet)
        {   
            _games[0].AddToPot(contribution);

            ChangeMoney(-contribution);
            _betMoney += contribution;

            status = ContestantStatus.Called;

            //Update the current bet
            _games[0].OnRaise(raisedBet);
        }
        else
        {
            Call();
        }
    } 

    //Get the raised bet from the manager, and find the difference
    public void Call()
    {
        //How much does the contestant have to bet to meet the call?
        //Debug.Log("Bet: " + _games[0].currentBet);
        //CONDITION: The bet should always be greater than the amount the contestant has already wagered
        int contribution = _games[0].currentBet - _betMoney;

        //If the contestant does not have enough money to meet the call, then offer whatever money the contestant has left
        if (contribution > _money)
            contribution = _money;

        //Also handles the edge case where the bet exceeds the contestant's available money
        _games[0].AddToPot(contribution);

        ChangeMoney(-contribution);
        _betMoney += contribution;

        status = ContestantStatus.Called;

        _games[0].OnCall();        
    }

    public void Check()
    {
        status = ContestantStatus.Called;
        _games[0].OnCheck();
    }

    public void Fold()
    {
        status = ContestantStatus.Folded;
        _games[0].OnFold(this);
    }

    public void GiveBlind(int blind)
    {
        int contribution = blind;

        if (blind > _money)
        {
            contribution = _money;
        }

        _games[0].AddToPot(contribution);

        ChangeMoney(-contribution);
        _betMoney = contribution;
    }

    public virtual void ChangeMoney(int money)
    {
        _money += money;
        if (_money < 0)
            _money = 0;
    }

    //Set the bet money to 0, typically at the start of a round
    public void ResetBet()
    {
        _betMoney = 0;
    }

    public void AssignScore(double score, string handName, int[] bestHand)
    {
        _handScore = score;
        _handName = handName;
        _bestPossibleHand = bestHand;
    }
    public abstract IPokerCommand MakeDecision();
}
