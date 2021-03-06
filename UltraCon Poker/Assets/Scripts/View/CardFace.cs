using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Every Card with have a face
public class CardFace : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    public Sprite[] faces;
    public Sprite cardBack;

    public int cardIndex;

    public void ToggleFace(bool showFace)
    {
        if (showFace)
            spriteRenderer.sprite = faces[cardIndex];
        else
            spriteRenderer.sprite = cardBack;
    }

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
}
