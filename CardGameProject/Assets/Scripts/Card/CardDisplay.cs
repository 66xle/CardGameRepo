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

        Name.text = Card.cardName;
        Description.text = Card.displayDescription;
        Flavour.text = Card.flavour;
        Cost.text = Card.cost.ToString();

        Image.sprite = Card.image;
        Frame.sprite = Card.frame;
    }

    public void SetCard(Card card)
    {
        Card = card;
    }
}
