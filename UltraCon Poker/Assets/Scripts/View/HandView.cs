using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Hand))]
public class HandView : MonoBehaviour
{
    Hand hand;
    Dictionary<int, CardView> fetchedCards;

    public Vector3 start;
    public float cardOffset;
    public bool faceUp = false;
    public GameObject cardPrefab;
    public GameObject cardHolder;

    public void Toggle (int card, bool isFaceUp)
    {
        fetchedCards[card].IsFaceUp = isFaceUp;
    }

    public void hand_Clear(object sender)
    {
        Clear();
    }
    public void Clear()
    {
        foreach(CardView view in fetchedCards.Values)
        {
            Destroy(view.Card);
        }
        fetchedCards.Clear();
    }

    private void Awake() {
        hand = GetComponent<Hand>();
        cardHolder = new GameObject("cardHolder");
        cardHolder.transform.position = start;
    }
    private void Start() {
        fetchedCards = new Dictionary<int, CardView>();

        hand.CardAdded += hand_CardAdded;
    }

    void hand_CardAdded(object sender, CardEventArgs e)
    {
        float co = cardOffset * hand.numCards;
        Vector3 temp = start + new Vector3(co, 0f);
        AddCard(temp, e.cardValue, e.cardSuit, hand.numCards);
    }

    private void Update() {
        //ShowCards();
    }

    public void ShowCards()
    {
        int cardCount = 0;
        if (hand.numCards > 0)
        {
            foreach(Card card in hand.cards)
            {
                float co = cardOffset * cardCount;
                Vector3 temp = start + new Vector3(co, 0f);
                AddCard(temp, card.value, card.suit, cardCount);

                cardCount++;
            }
        }
    }

    //Add a visual card to the hand 
    void AddCard(Vector3 position, CardValue value, CardSuit suit, int positionIndex)
    {
        //Determine the index of the cardface in the CardFace array from the value and suit

        GameObject cardCopy = (GameObject)Instantiate(cardPrefab);
        cardCopy.transform.position = position;

        int cardIndex = GetCardIndex(value, suit);

        CardFace cardModel = cardCopy.GetComponent<CardFace>();
        cardModel.cardIndex = cardIndex;    
        cardModel.ToggleFace(faceUp);

        //Cannot use new
        fetchedCards.Add(cardIndex, new CardView(cardCopy));
        cardCopy.transform.parent = cardHolder.transform;
    }

    int GetCardIndex(CardValue value, CardSuit suit)
    {
        return ((int) value * 4) + ((int) suit);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(start, new Vector3(50, 75, 1));
    }

}
