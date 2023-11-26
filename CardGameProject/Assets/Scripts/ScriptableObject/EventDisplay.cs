using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventDisplay : MonoBehaviour
{
    public Image image;
    public TMP_Text description;

    public void Display(Event eventObj)
    {
        image.sprite = eventObj.image;
        description.text = eventObj.description;
    }

    
}
