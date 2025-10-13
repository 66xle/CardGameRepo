using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private Camera UICamera;
    private bool _isPopupDisabled = false;

    public Card Card { get; private set; }
    public CardData CardData { get; private set; }

    private void Update()
    {
        if (InputManager.Instance.LeftClickInputDown && !_isPopupDisabled)
        {
            if (Description.textInfo.linkInfo.Count() <= 0) return;


            int linkIndex = TMP_TextUtilities.FindIntersectingLink(Description, Input.mousePosition, UICamera);
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

    public void Init()
    {
        Name.text = Card.CardName;
        Description.text = Card.DisplayDescription;
        Flavour.text = Card.Flavour;
        Cost.text = Card.Cost.ToString();

        Image.sprite = Card.Image;
        Frame.sprite = Card.Frame;
    }

    public void SetCard(CardData cardData, Card card)
    {
        Card = card;
        CardData = cardData;
        Init();
    }

    public void ClosePopup()
    {
        PopupObj.SetActive(false);
    }

    public void DisablePopup()
    {
        _isPopupDisabled = true;
    }

    public void UpdateDescription(string description)
    {
        Description.text = description;
    }

    public void SetCamera(Camera camera)
    {
        UICamera = camera;
    }
}
