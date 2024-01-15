using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public Card card;

    public TMP_Text name;
    public TMP_Text description;
    public TMP_Text flavour;
    public TMP_Text cost;
    public Image image;
    public Image frame;

    private void Start()
    {
        name.text = card.name;
        description.text = card.description;
        flavour.text = card.flavour;
        cost.text = card.cost.ToString();

        image.sprite = card.image;
        frame.sprite = card.frame;
    }
}
