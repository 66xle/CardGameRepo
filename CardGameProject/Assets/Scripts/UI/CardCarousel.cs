using config;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using demo;
using events;

public class CardCarousel : MonoBehaviour
{
    [Header("Constraints")]
    [SerializeField] private bool forceFitContainer;

    [Header("Push")]
    [SerializeField] private float pushAmount = 100f;
    [SerializeField] private float falloff = 0.5f;
    [SerializeField][Tooltip("Cards adjacent affected by push")] private int affectedCardCount = 2;
    [SerializeField][Tooltip("Number of cards in hand to be affected by push")] private int pushAffectedCardCount = 2;
    [SerializeField] private float rightPushMultiplier = 1.25f;

    [Header("Alignment")]
    [SerializeField]
    private CardAlignment alignment = CardAlignment.Center;

    [SerializeField]
    private bool allowCardRepositioning = true;

    [SerializeField]
    private ZoomConfig zoomConfig;

    [SerializeField]
    private AnimationSpeedConfig animationSpeedConfig;

    [SerializeField]
    private CardPlayConfig cardPlayConfig;

    [Header("Events")]
    [SerializeField]
    private EventsConfig eventsConfig;

    private List<CardCarouselDisplay> cards = new List<CardCarouselDisplay>();

    private RectTransform rectTransform;
    private CardCarouselDisplay currentDraggedCard;
    private CardCarouselDisplay currentSelectedCard;
    private bool isDragging = false;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        InitCards();
    }

    private void InitCards()
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

            AddOtherComponentsIfNeeded(wrapper);

            // Pass child card any extra config it should be aware of
            wrapper.zoomConfig = zoomConfig;
            wrapper.animationSpeedConfig = animationSpeedConfig;
            wrapper.eventsConfig = eventsConfig;
            wrapper.container = this;
            wrapper.card = card.GetComponent<CardDisplay>().Card;
        }
    }

    private void SetCardsAnchor()
    {
        foreach (CardCarouselDisplay child in cards)
        {
            child.SetAnchor(new Vector2(0, 0.5f), new Vector2(0, 0.5f));
        }
    }

    private void AddOtherComponentsIfNeeded(CardCarouselDisplay wrapper)
    {
        var canvas = wrapper.GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = wrapper.gameObject.AddComponent<Canvas>();
        }

        canvas.overrideSorting = true;

        if (wrapper.GetComponent<GraphicRaycaster>() == null)
        {
            wrapper.gameObject.AddComponent<GraphicRaycaster>();
        }
    }

    void Update()
    {
        UpdateCards();
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
        UpdateCardOrder();
    }

    private void SetCardsPosition()
    {
        // Compute the total width of all the cards in global space
        var cardsTotalWidth = cards.Sum(card => card.width);
        // Compute the width of the container in global space
        var containerWidth = rectTransform.rect.width * transform.lossyScale.x;
        if (forceFitContainer && cardsTotalWidth > containerWidth)
        {
            DistributeChildrenToFitContainer(cardsTotalWidth);
        }
        else
        {
            DistributeChildrenWithoutOverlap(cardsTotalWidth);
        }
    }

    private void SetCardsUILayers()
    {
        for (var i = 0; i < cards.Count; i++)
        {
            cards[i].uiLayer = zoomConfig.defaultSortOrder + i;
        }
    }

    private void UpdateCardOrder()
    {
        if (!allowCardRepositioning || currentDraggedCard == null) return;

        // Get the index of the dragged card depending on its position
        var newCardIdx = cards.Count(card => currentDraggedCard.transform.position.x > card.transform.position.x);
        var originalCardIdx = cards.IndexOf(currentDraggedCard);
        if (newCardIdx != originalCardIdx)
        {
            cards.RemoveAt(originalCardIdx);
            if (newCardIdx > originalCardIdx && newCardIdx < cards.Count - 1)
            {
                newCardIdx--;
            }

            cards.Insert(newCardIdx, currentDraggedCard);
        }
        // Also reorder in the hierarchy
        currentDraggedCard.transform.SetSiblingIndex(newCardIdx);
    }

    private void DistributeChildrenToFitContainer(float childrenTotalWidth)
    {
        int selectedIndex = currentSelectedCard == null ? -1 : cards.IndexOf(currentSelectedCard);

        // Get the width of the container
        var width = rectTransform.rect.width * transform.lossyScale.x;
        // Get the distance between each child
        var distanceBetweenChildren = (width - childrenTotalWidth) / (cards.Count - 1);
        // Set all children's positions to be evenly spaced out
        var currentX = transform.position.x - width / 2;

        for (int i = 0; i < cards.Count; i++)
        {
            var child = cards[i];
            float adjustedChildWidth = child.width;

            // Base X without push
            float baseX = currentX + adjustedChildWidth / 2;

            child.targetPosition = new Vector2(baseX, transform.position.y);
            currentX += adjustedChildWidth + distanceBetweenChildren;
        }
    }

    private void DistributeChildrenWithoutOverlap(float childrenTotalWidth)
    {
        var currentPosition = GetAnchorPositionByAlignment(childrenTotalWidth);
        foreach (CardCarouselDisplay child in cards)
        {
            var adjustedChildWidth = child.width;
            child.targetPosition = new Vector2(currentPosition + adjustedChildWidth / 2, transform.position.y);
            currentPosition += adjustedChildWidth;
        }
    }

    private float GetAnchorPositionByAlignment(float childrenWidth)
    {
        var containerWidthInGlobalSpace = rectTransform.rect.width * transform.lossyScale.x;
        switch (alignment)
        {
            case CardAlignment.Left:
                return transform.position.x - containerWidthInGlobalSpace / 2;
            case CardAlignment.Center:
                return transform.position.x - childrenWidth / 2;
            case CardAlignment.Right:
                return transform.position.x + containerWidthInGlobalSpace / 2 - childrenWidth;
            default:
                return 0;
        }
    }

    public void OnCardDragStart(CardCarouselDisplay card)
    {
        isDragging = true;
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
        currentSelectedCard = card;
    }
}
