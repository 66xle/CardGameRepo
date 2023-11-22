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
    public TMP_Text cost;
    public Image image;

    private void Start()
    {
        name.text = card.name;
        description.text = card.description;
        cost.text = card.cost.ToString();

        image.sprite = card.image;
    }
}
