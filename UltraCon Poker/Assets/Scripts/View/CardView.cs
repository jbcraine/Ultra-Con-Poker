using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A CardView will alwayts be associated with a Card object
public class CardView : MonoBehaviour
{
    public GameObject Card { get; private set; }
    public bool IsFaceUp { get; set; }

    public CardView(GameObject card)
    {
        Card = card;
        IsFaceUp = false;
    }
}
