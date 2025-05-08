using System.Collections;
using System.Collections.Generic;
using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    [MustBeAssigned][SerializeField] TMP_Text Name;
    [MustBeAssigned] [SerializeField] TMP_Text Description;
    [MustBeAssigned] [SerializeField] TMP_Text Flavour;
    [MustBeAssigned] [SerializeField] TMP_Text Cost;
    [MustBeAssigned] [SerializeField] Image Image;
    [MustBeAssigned] [SerializeField] Image Frame;
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
