using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Hand : MonoBehaviour{

    public int numCards {get; private set;}
    public int maxCards {get; private set;}
    [SerializeField] private List<Card> _cards;
    private Dictionary<CardValue, List<Card>> _ranks;
    private Dictionary<CardSuit, List<Card>> _suits;
    private HandView _view;

    public event CardEventHandler CardRemoved;
    public event CardEventHandler CardAdded;
    public event ClearHandHandler HandCleared;

    private void Awake() {
        _view = GetComponent<HandView>();
        if (_view != null)
        {
            HandCleared += _view.hand_Clear;
        }
        _cards = new List<Card>();

        _ranks = new Dictionary<CardValue, List<Card>>();
        for (int i = 0; i < 13; ++i)
            _ranks.Add((CardValue) i, new List<Card>());
    
        _suits = new Dictionary<CardSuit, List<Card>>();
        for (int i = 0; i < 4; ++i)
            _suits.Add((CardSuit) i, new List<Card>());
    }

    public List<Card> cards
    {
        get {return _cards;}
    }

    public void SetMaxCards(int maxCards)
    {
        this.maxCards = maxCards;
    }

    //Update the hand by adding a new card, and update the bins for keeping track of ranks and suits
    public void AddCard(Card card)
    {
        if (numCards++ < maxCards)
        {
            cards.Add(card);

            //Signal that a card has been added to the hand. Notify the view
            if (CardAdded != null)
            {
                CardAdded(this, new CardEventArgs(card.value, card.suit));
            }

            _ranks[card.value].Add(card);
            _suits[card.suit].Add(card);
        }
    }

    //Return the number of cards with the given rank
    public int GetRankCount(CardValue rank)
    {
        List<Card> value;
        _ranks.TryGetValue(rank, out value);
        return value.Count;
    }

    //Return the number of cards with the given suit
    public int GetSuitCount(CardSuit suit)
    {
        List<Card> value;
        _suits.TryGetValue(suit, out value);
        return value.Count;
    }

    public bool HasRank(CardValue rank)
    {
        List<Card> value;
        _ranks.TryGetValue(rank, out value);
        return (value.Count > 0);
    }

    public bool HasSuit(CardSuit suit)
    {
        List<Card> value;
        _suits.TryGetValue(suit, out value);
        return (value.Count > 0);
    }

    //Use a simple Insertion Sort to sort the cards
    public void SortHand()
    {
        Card key;
        int i, j;
        for (i = 1; i < cards.Count; ++i)
        {
            key = cards[i];
            j = i - 1;

            while (j >= 0 && cards[j].value > key.value)
            {
                cards[j + 1] = cards[j];
                j = j - 1;
            }
            cards[j + 1] = key;
        }
    }

    public void Clear()
    {
        cards.Clear();
        foreach (KeyValuePair<CardValue, List<Card>> kvp in _ranks)
        {
            kvp.Value.Clear();
        }
        foreach(KeyValuePair<CardSuit, List<Card>> kvp in _suits)
        {
            kvp.Value.Clear();
        }
        numCards = 0;
        HandCleared(this);
    }

}
