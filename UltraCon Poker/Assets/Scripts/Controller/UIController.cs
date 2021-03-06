using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIController : MonoBehaviour
{
    [SerializeField] private PlayerContestant player;
    [SerializeField] private PokerManager game;
    public Button checkButton, raiseButton, foldButton;
    public Button startGameButton, startRoundButton;
    //The maximum value will need to be changed as the player's money changes
    public Slider moneySlider;
    public Text callAndRaiseText, availableMoney, callAndCheckButtonText, currentBetText, currentPotText;
    public Text winnerText, winningHandText;
    public Text contestantEliminatedText;
    private int currentMoney;
    private int currentBet;
    private int currentCall;
    private int currentPot;
    //This will be the max value of the raise slider
    private int roundStartingMoney;
    
    private void Awake() {
        player.MoneyChanged += OnMoneyChanged;
        game.BetChanged += OnBetChanged;
        game.PotChanged += OnPotChanged;
        game.RoundStarted += OnRoundStarted;
        game.ContestantEliminated += OnContestantEliminated;
        game.PotWon += OnPotWon;
        game.RoundEnded += OnRoundEnded;
        game.CalledAllContestants += OnAllContestantsCalled;
        game.MatchEnded += OnMatchEnded;
    }

    //Activate the UI when it is the player's turn
    public void ActivateUI()
    {
        checkButton.interactable = true;
        raiseButton.interactable = true;
        foldButton.interactable = true;
        moneySlider.interactable = true;
    }

    //Deactivate the UI when the player finishes their turn
    public void DeactivateUI()
    {
        checkButton.interactable = false;
        raiseButton.interactable = false;
        foldButton.interactable = false;
        moneySlider.interactable = false;
    }

    public void OnCall()
    {
        IPokerCommand command = null;
        if (game.isPlayerTurn)
        {
            if((player.money + player.betMoney) >= currentCall)
                command = new CallCommand(player);
            else
                command = new CallCommand(player);
            DeactivateUI();
        }
        
        command.Execute();
    }

    public void OnFold()
    {
        IPokerCommand command = null;
        if (game.isPlayerTurn)
        {
            command = new FoldCommand(player);
            DeactivateUI();
        }

        command.Execute();
    }

    public void OnRaise()
    {
        IPokerCommand command = null;
        if (game.isPlayerTurn)
        {
            command = new RaiseCommand(player, currentBet);
            DeactivateUI();
        }

        command.Execute();
    }

    public void OnStartGame()
    {
        game.StartMatch();
        startGameButton.interactable = false;
    }

    public void OnStartRound()
    {
        game.StartNextRound();
        startRoundButton.interactable = false;
    }

    public void OnMoneyValue(float value)
    {
        //Value should adjust in increments of 10
        float adjustedValue = (value / 10) * 10;

        //moneySlider.value = adjustedValue;
        //currentBet = (int) adjustedValue;
        currentBet = (int) value;
        callAndRaiseText.text = "$" + adjustedValue;
    }


    void OnMoneyChanged(object sender, MoneyEventArgs e)
    {
        //Debug.Log(e.money);
        currentMoney = e.money;
        availableMoney.text = "$" + currentMoney.ToString();
    }

    void OnBetChanged(object sender, BetEventArgs b)
    {
        Debug.Log(b.currentBet);
        currentCall = b.currentBet;
        moneySlider.minValue = currentCall;
        callAndRaiseText.text = "$" + currentCall.ToString();
        callAndCheckButtonText.text = "Call";
        currentBetText.text = "Current Bet: $" + currentCall.ToString();
    }

    void OnPotChanged(object sender, PotChangedEventArgs p)
    {
        currentPot = p._pot;
        currentPotText.text = "Current Pot: $" + currentPot.ToString();
    }

    void OnRoundStarted(object sender, RoundStartedEventArgs r)
    {
        roundStartingMoney = r._startingMoney;
        moneySlider.maxValue = roundStartingMoney;
    }

    void OnPotWon(object sender, WonPotEventArgs w)
    {
        //handName will be null if all by one contestant folds

        winnerText.text = w.winnerName + " Wins The Round!";
        winnerText.gameObject.SetActive(true);

        if (w.showDownHappened)
        {
            if (w.showDownDrawed)
            {
                winnerText.text = "Draw!";
                winningHandText.text = w.handName;
            }
            else
            {
                winningHandText.text = w.handName;
            }
                
            winningHandText.gameObject.SetActive(true);
        }
    }

    void OnContestantEliminated (object sender, ContestantEliminatedEventArgs c)
    {
        winningHandText.gameObject.SetActive(false);
        contestantEliminatedText.text = c.contestantName + " has been eliminated";
    }

    void OnRoundEnded (object sender) 
    {
        winnerText.gameObject.SetActive(false);
        winningHandText.gameObject.SetActive(false);
        startRoundButton.interactable = true;
    }

    void OnAllContestantsCalled (object sender)
    {
        callAndCheckButtonText.text = "Check";
    }

    void OnMatchEnded (object sender, MatchEndEventArgs m)
    {
        winnerText.text = m.winnerName + "Has Won the Game!";
        startGameButton.interactable = true;
        startRoundButton.interactable = false;
    }
}
