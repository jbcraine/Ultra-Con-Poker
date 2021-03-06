using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    private const int STANDARD_DECK_SIZE = 52;
    private const int TOTAL_VALUES = 13;
    private const int TOTAL_SUITS = 4;
    [SerializeField] private int _deckSize = STANDARD_DECK_SIZE;
    [SerializeField] private List<Card> _deck;
    private int _deckRemaining;

    //Produce a standard deck of 52 cards
    private void Start() 
    {
        _deckSize = STANDARD_DECK_SIZE;
        _deckRemaining = _deckSize;
        _deck = new List<Card>();
    }

    public void FillStandardDeck()
    {
        _deck.Clear();
        
        for (int suit = 0; suit < TOTAL_SUITS; ++suit)
        {
            for (int value = 0; value < TOTAL_VALUES; ++value)
            {
                Card card = new Card((CardValue) value, (CardSuit) suit);
                _deck.Add(card);
            }
        }
        _deckRemaining = STANDARD_DECK_SIZE;
    }

    //Remove all cards from the deck with the given value by replacing them with a null value
    public void RemoveCardValue(CardValue value)
    {
        _deck.RemoveAll(CardHasValue);

        bool CardHasValue(Card card)
        {
            return card.value == value;
        }
    }

    //Remove all cards from the deck with the given suit
    public void RemoveCardSuit(CardSuit suit)
    {
        _deck.RemoveAll(CardHasSuit);

        bool CardHasSuit(Card card)
        {
            return card.suit == suit;
        }
    }

    //Change all cards with the given value to have a new value
    public void ChangeCardValue(CardValue value, CardValue newValue)
    {
        _deck.ForEach(CardHasValue);

        void CardHasValue(Card card)
        {
            if (card.value == newValue)
                card.ChangeValue(newValue);
        }
    }

    //Change all cards with the given suit to have a new suit
    public void ChangeCardSuit(CardSuit suit, CardSuit newSuit)
    {
        _deck.ForEach(CardHasSuit);

        void CardHasSuit(Card card)
        {
            if (card.suit == suit)
                card.ChangeSuit(newSuit);
        }
    }

    //Shuffle the deck with a version of the Knute shuffle algorithm, which swaps at each index with another random index 
    public void ShuffleDeck()
    {
        for (int i = 0; i < _deckSize; ++i)
        {
            Card temp;
            temp = _deck[i];
            int randomIndex = Random.Range(0, _deckSize);
            _deck[i] = _deck[randomIndex];
            _deck[randomIndex] = temp;
        }
    }

    //Return the card at the top of the deck and decrease its size
    public Card PopCard()
    {
        if (_deckRemaining != 0)
        {
            if (_deck[_deckRemaining - 1] != null)
            {
                _deckRemaining -= 1;
                return _deck[_deckRemaining];
            }
        }
        return null;
    }

    
}