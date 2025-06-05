using System.Collections;
using System.Collections.Generic;
using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    [Header("Card")]
    [MustBeAssigned] [SerializeField] TMP_Text Name;
    [MustBeAssigned] [SerializeField] TMP_Text Description;
    [MustBeAssigned] [SerializeField] TMP_Text Flavour;
    [MustBeAssigned] [SerializeField] TMP_Text Cost;
    [MustBeAssigned] [SerializeField] Image Image;
    [MustBeAssigned] [SerializeField] Image Frame;

    [Header("Popup")]
    [MustBeAssigned] [SerializeField] GameObject PopupObj;
    [MustBeAssigned] [SerializeField] TMP_Text PopupTitle;
    [MustBeAssigned] [SerializeField] TMP_Text PopupDescription;


    public Card Card { get; private set; }

    private void Start()
    {
        if (Card == null) return;

        Name.text = Card.CardName;
        Description.text = Card.LinkDescription;
        Flavour.text = Card.Flavour;
        Cost.text = Card.Cost.ToString();

        Image.sprite = Card.Image;
        Frame.sprite = Card.Frame;
    }

    private void Update()
    {
        if (InputManager.Instance.LeftClickInputDown)
        {
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(Description, Input.mousePosition, null);
            if (linkIndex != -1)
            {
                TMP_LinkInfo linkInfo = Description.textInfo.linkInfo[linkIndex];
                string linkID = linkInfo.GetLinkID();

                PopupObj.SetActive(true);

                for (int i = 0; i < Card.PopupKeyPair.Count; i++)
                {
                    if (Card.PopupKeyPair[i].Key != linkID) continue;
                    
                    PopupTitle.text = Card.PopupKeyPair[i].Value.Title;
                    PopupDescription.text = Card.PopupKeyPair[i].Value.DisplayDescription;
                }
            }
        }
    }

    public void SetCard(Card card)
    {
        Card = card;
    }

    public void ClosePopup()
    {
        PopupObj.SetActive(false);
    }
}
