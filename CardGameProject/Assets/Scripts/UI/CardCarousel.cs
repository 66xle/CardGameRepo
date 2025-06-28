using config;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using demo;
using events;
using MyBox;
using TMPro;

public class CardCarousel : MonoBehaviour
{
    [MustBeAssigned] [SerializeField] GameObject InfoBoxObj;
    private RectTransform InfoBoxRect;
    [MustBeAssigned] [SerializeField] TMP_Text InfoTitle;
    [MustBeAssigned] [SerializeField] TMP_Text InfoDescription;
    [MustBeAssigned] [SerializeField] TMP_Text InfoFlavourText;

    [MustBeAssigned] [SerializeField] GameObject PopupObj;
    private RectTransform PopupRect;
    [MustBeAssigned] [SerializeField] TMP_Text PopupTitle;
    [MustBeAssigned] [SerializeField] TMP_Text PopupDescription;

    [SerializeField]
    private ZoomConfig zoomConfig;

    [SerializeField]
    private AnimationSpeedConfig animationSpeedConfig;

    [SerializeField]
    private CardPlayConfig cardPlayConfig;

    private List<CardCarouselDisplay> cards = new List<CardCarouselDisplay>();

    private bool wasCardClickedThisFrame = false;

    private CardCarouselDisplay currentSelectedCard;
    private int lastSiblingIndex;
    private bool isCardClicked = false;
    private int selectedCardIndex = -1;
    private bool snapToSelected = false;

    private CardCarouselDisplay currentDraggedCard;
    private bool isDragging = false;
    private Vector2 dragEventData;
    private float scrollOffsetX = 0f;         // Persistent total offset (includes snapping)
    private float dragStartPointerX = 0f;     // Pointer x position at drag start
    private bool wasDraggingLastFrame = false;
    private float dragOffsetX;


    private void Start()
    {
        InitCards();

        InfoBoxRect = InfoBoxObj.GetComponent<RectTransform>();
        PopupRect = PopupObj.GetComponent<RectTransform>();
    }

    public void InitCards()
    {
        SetUpCards();
        SetCardsAnchor();
    }

    void SetUpCards()
    {
        cards.Clear();
        foreach (Transform card in transform)
        {
            var wrapper = card.GetComponent<CardCarouselDisplay>();
            if (wrapper == null)
            {
                wrapper = card.gameObject.AddComponent<CardCarouselDisplay>();
            }

            cards.Add(wrapper);

            // Pass child card any extra config it should be aware of
            wrapper.zoomConfig = zoomConfig;
            wrapper.animationSpeedConfig = animationSpeedConfig;
            wrapper.container = this;

            CardDisplay display = card.GetComponent<CardDisplay>();
            display.DisablePopup();

            wrapper.card = display.Card;
        }
    }

    private void SetCardsAnchor()
    {
        foreach (CardCarouselDisplay child in cards)
        {
            child.SetAnchor(new Vector2(0, 0.5f), new Vector2(0, 0.5f));
        }
    }


    void Update()
    {
        UpdateCards();
        UpdateLink();

        if (InputManager.Instance.LeftClickInputUp)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(InfoBoxRect, Input.mousePosition))
                return;

            if (RectTransformUtility.RectangleContainsScreenPoint(PopupRect, Input.mousePosition))
                return;

            if (!isCardClicked && currentSelectedCard != null)
            {
                CardPreviewEnd();
            }

            // Reset click flag every mouse up
            isCardClicked = false;
        }
    }

    private void UpdateLink()
    {
        if (InputManager.Instance.LeftClickInputDown)
        {
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(InfoDescription, Input.mousePosition, null);
            if (linkIndex != -1)
            {
                TMP_LinkInfo linkInfo = InfoDescription.textInfo.linkInfo[linkIndex];
                string linkID = linkInfo.GetLinkID();

                PopupObj.SetActive(true);

                for (int i = 0; i < currentSelectedCard.card.PopupKeyPair.Count; i++)
                {
                    if (currentSelectedCard.card.PopupKeyPair[i].Key != linkID) continue;

                    PopupTitle.text = currentSelectedCard.card.PopupKeyPair[i].Value.Title;
                    PopupDescription.text = currentSelectedCard.card.PopupKeyPair[i].Value.DisplayDescription;
                }
            }
        }
    }

    private void UpdateCards()
    {
        if (transform.childCount != cards.Count)
        {
            InitCards();
        }

        if (cards.Count == 0)
        {
            return;
        }

        SetCardsPosition();
        SetCardsUILayers();
    }

    private void SetCardsPosition()
    {
        // Compute the total width of all the cards in global space
        var cardsTotalWidth = cards.Sum(card => card.width);
        DistributeChildrenWithoutOverlap(cardsTotalWidth);
    }

    private void SetCardsUILayers()
    {
        for (var i = 0; i < cards.Count; i++)
        {
            cards[i].uiLayer = i;
        }
    }

    private void DistributeChildrenWithoutOverlap(float childrenTotalWidth)
    {
        float containerCenterX = transform.position.x;
        float anchorPosition = transform.position.x - childrenTotalWidth / 2;

        if (isDragging)
        {
            if (!wasDraggingLastFrame)
            {
                // Just started dragging — record starting pointer position
                dragStartPointerX = Input.mousePosition.x;

                scrollOffsetX = dragOffsetX;
            }

            // Live offset based on current pointer vs start
            float dragDelta = Input.mousePosition.x - dragStartPointerX;
            dragOffsetX = scrollOffsetX + dragDelta;
        }

        if (snapToSelected && selectedCardIndex >= 0 && selectedCardIndex < cards.Count)
        {
            float offset = 0f;
            for (int i = 0; i < selectedCardIndex; i++)
                offset += cards[i].width;

            float selectedCardCenter = anchorPosition + scrollOffsetX + offset + cards[selectedCardIndex].width / 2f;

            float snapOffset = containerCenterX - selectedCardCenter;

            scrollOffsetX += snapOffset;
            dragOffsetX = scrollOffsetX;

            snapToSelected = false;
        }

        // Apply layout
        float currentX = anchorPosition + dragOffsetX;
        foreach (var child in cards)
        {
            float w = child.width;
            float xPos = currentX + w / 2f;
            child.targetPosition = new Vector2(xPos, transform.position.y);
            currentX += w;
        }

        wasDraggingLastFrame = isDragging;
    }

    private void CardPreviewEnd()
    {
        currentSelectedCard.IsPreviewActive = false;
        currentSelectedCard.transform.SetSiblingIndex(lastSiblingIndex);
        currentSelectedCard = null;

        selectedCardIndex = -1;
        snapToSelected = false;
        InfoBoxObj.SetActive(false);
        PopupObj.SetActive(false);
    }

    public void OnCardDragStart(CardCarouselDisplay card, Vector2 dragEventData)
    {
        if (currentSelectedCard != null)
            CardPreviewEnd();

        isDragging = true;
        this.dragEventData = dragEventData;
        Debug.Log("is dragging");
    }

    public void OnCardDragEnd()
    {
        isDragging = false;

        Debug.Log("stop dragging");

        if (currentDraggedCard == null)
            return;

        currentDraggedCard = null;
    }

    public void OnClickStart(CardCarouselDisplay card)
    {
        if (currentSelectedCard != null)
            CardPreviewEnd();

        PopupObj.SetActive(false);

        isCardClicked = true;
        InfoBoxObj.SetActive(true);
        InfoTitle.text = card.card.CardName;
        InfoDescription.text = card.card.LinkDescription;
        InfoFlavourText.text = card.card.Flavour;

        currentSelectedCard = card;
        lastSiblingIndex = currentSelectedCard.transform.GetSiblingIndex();
        currentSelectedCard.transform.SetSiblingIndex(currentSelectedCard.transform.parent.childCount - 1);
        currentSelectedCard.IsPreviewActive = true;


        selectedCardIndex = cards.IndexOf(card);
        if (selectedCardIndex != -1)
            snapToSelected = true;
    }
}
