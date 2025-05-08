using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text Name;
    [SerializeField] TMP_Text Description;
    [SerializeField] TMP_Text Flavour;
    [SerializeField] TMP_Text Cost;
    [SerializeField] Image Image;
    [SerializeField] Image Frame;
    public Card Card { get; private set; }

    private void Start()
    {
        Card.GenerateDisplayDescription();

        Name.text = Card.CardName;
        Description.text = Card.DisplayDescription;
        Flavour.text = Card.Flavour;
        Cost.text = Card.Cost.ToString();

        Image.sprite = Card.Image;
        Frame.sprite = Card.Frame;
    }

    public void SetCard(Card card)
    {
        Card = card;
    }
}
