﻿using config;
using DefaultNamespace;
using demo;
using events;
using MyBox;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CardContainer : MonoBehaviour {
    [Header("References")]
    [MustBeAssigned] public CombatStateMachine combatStateMachine;
    [MustBeAssigned] public ClonePreviewManager clonePreviewManager;

    [Header("Constraints")]
    [SerializeField] private bool forceFitContainer;

    [Header("Clone Preview")]
    [SerializeField] private float pushAmount = 100f;
    [SerializeField] private float falloff = 0.5f;
    [SerializeField] [Tooltip("Cards adjacent affected by push")] private int affectedCardCount = 2;
    [SerializeField][Tooltip("Number of cards in hand to be affected by push")] private int pushAffectedCardCount = 2;
    [SerializeField] private float rightPushMultiplier = 1.25f;

    [Header("Alignment")]
    [SerializeField]
    private CardAlignment alignment = CardAlignment.Center;

    [SerializeField]
    private bool allowCardRepositioning = true;

    [Header("Rotation")]
    [SerializeField]
    [Range(0f, 90f)]
    private float maxCardRotation;

    
    [SerializeField] float maxHeightDisplacement;
    [SerializeField] float minRatio = 0.3f;


    [Header("Play Delta")]
    [SerializeField] float YAmount = 0.3f;

    [SerializeField]
    private ZoomConfig zoomConfig;

    [SerializeField]
    private AnimationSpeedConfig animationSpeedConfig;

    [SerializeField]
    private CardPlayConfig cardPlayConfig;
    
    [Header("Events")]
    [SerializeField]
    private EventsConfig eventsConfig;

    private List<CardWrapper> cards = new List<CardWrapper>();

    private RectTransform rectTransform;
    [HideInInspector] public CardWrapper currentDraggedCard;

    private void Start() {
        rectTransform = GetComponent<RectTransform>();
        InitCards();
    }

    private void InitCards() {
        SetUpCards();
        SetCardsAnchor();
    }

    private void SetCardsRotation() {
        for (var i = 0; i < cards.Count; i++) {
            cards[i].targetRotation = GetCardRotation(i);
            cards[i].targetVerticalDisplacement = GetCardVerticalDisplacement(i);
        }
    }

    private float GetCardVerticalDisplacement(int index)
    {
        float baseHeight = 1080f;
        float screenRatio = Screen.height / baseHeight;
        float scaledOffsetY = maxHeightDisplacement * screenRatio;

        if (cards.Count <= 1)
            return scaledOffsetY; // fully centered, no curve

        float center = (cards.Count - 1) / 2f;
        float normalizedDistance = (index - center) / center; // -1 to 1
        float curve = 1 - Mathf.Pow(normalizedDistance, 2);   // parabolic

        float finalRatio = Mathf.Lerp(minRatio, 1f, curve);
        return scaledOffsetY * finalRatio;
    }

    private float GetCardRotation(int index) {
        if (cards.Count < 3) return 0;
        // Associate a rotation based on the index in the cards list
        // so that the first and last cards are at max rotation, mirrored around the center
        return -maxCardRotation * (index - (cards.Count - 1) / 2f) / ((cards.Count - 1) / 2f);
    }

    void Update() {
        UpdateCards();
    }

    void SetUpCards() {
        cards.Clear();
        foreach (Transform card in transform) {
            var wrapper = card.GetComponent<CardWrapper>();
            if (wrapper == null) {
                wrapper = card.gameObject.AddComponent<CardWrapper>();
            }

            cards.Add(wrapper);

            AddOtherComponentsIfNeeded(wrapper);

            // Pass child card any extra config it should be aware of
            wrapper.zoomConfig = zoomConfig;
            wrapper.animationSpeedConfig = animationSpeedConfig;
            wrapper.eventsConfig = eventsConfig;
            wrapper.container = this;
            wrapper.card = card.GetComponent<CardDisplay>().Card;
            wrapper.combatStateMachine = combatStateMachine;
        }
    }

    private void AddOtherComponentsIfNeeded(CardWrapper wrapper) {
        var canvas = wrapper.GetComponent<Canvas>();
        if (canvas == null) {
            canvas = wrapper.gameObject.AddComponent<Canvas>();
        }

        canvas.overrideSorting = true;

        if (wrapper.GetComponent<GraphicRaycaster>() == null) {
            wrapper.gameObject.AddComponent<GraphicRaycaster>();
        }
    }

    private void UpdateCards() {
        if (transform.childCount != cards.Count) {
            InitCards();
        }

        if (cards.Count == 0) {
            return;
        }

        SetCardsPosition();
        SetCardsRotation();
        SetCardsUILayers();
        UpdateCardOrder();
    }

    private void SetCardsUILayers() {
        for (var i = 0; i < cards.Count; i++) {
            cards[i].uiLayer = zoomConfig.defaultSortOrder + i;
        }
    }

    private void UpdateCardOrder() {
        if (!allowCardRepositioning || currentDraggedCard == null) return;

        // Get the index of the dragged card depending on its position
        var newCardIdx = cards.Count(card => currentDraggedCard.transform.position.x > card.transform.position.x);
        var originalCardIdx = cards.IndexOf(currentDraggedCard);
        if (newCardIdx != originalCardIdx) {
            cards.RemoveAt(originalCardIdx);
            if (newCardIdx > originalCardIdx && newCardIdx < cards.Count - 1) {
                newCardIdx--;
            }

            cards.Insert(newCardIdx, currentDraggedCard);
        }
        // Also reorder in the hierarchy
        currentDraggedCard.transform.SetSiblingIndex(newCardIdx);
    }

    private void SetCardsPosition() {
        // Compute the total width of all the cards in global space
        var cardsTotalWidth = cards.Sum(card => card.width * card.transform.lossyScale.x);
        // Compute the width of the container in global space
        var containerWidth = rectTransform.rect.width * transform.lossyScale.x;
        if (forceFitContainer && cardsTotalWidth > containerWidth) {
            DistributeChildrenToFitContainer(cardsTotalWidth);
        }
        else {
            DistributeChildrenWithoutOverlap(cardsTotalWidth);
        }
    }

    private void DistributeChildrenToFitContainer(float childrenTotalWidth) {

        int selectedIndex = clonePreviewManager.currentCard == null ? -1 : cards.IndexOf(clonePreviewManager.currentCard);

        // Get the width of the container
        var width = rectTransform.rect.width * transform.lossyScale.x;
        // Get the distance between each child
        var distanceBetweenChildren = (width - childrenTotalWidth) / (cards.Count - 1);
        // Set all children's positions to be evenly spaced out
        var currentX = transform.position.x - width / 2;

        for (int i = 0; i < cards.Count; i++)
        {
            var child = cards[i];
            float adjustedChildWidth = child.width * child.transform.lossyScale.x;

            // Base X without push
            float baseX = currentX + adjustedChildWidth / 2;

            // Apply push if any card is selected
            float pushOffset = 0f;
            if (selectedIndex >= 0 && cards.Count >= pushAffectedCardCount)
            {
                int distanceFromSelected = Mathf.Abs(i - selectedIndex);

                if (distanceFromSelected <= affectedCardCount && i != selectedIndex)
                {
                    // Normalized falloff: closer cards are pushed more
                    float t = 1f - (distanceFromSelected / (float)(affectedCardCount + 1));
                    t = Mathf.Pow(t, falloff);

                    // Right side bias: push cards on the right harder
                    if (i > selectedIndex)
                    {
                        t *= rightPushMultiplier; // e.g., 1.25f
                    }

                    float direction = Mathf.Sign(i - selectedIndex); // -1 = left, +1 = right
                    pushOffset = direction * pushAmount * t;
                }
            }

            child.targetPosition = new Vector2(baseX + pushOffset, transform.position.y);
            currentX += adjustedChildWidth + distanceBetweenChildren;
        }
    }

    private void DistributeChildrenWithoutOverlap(float childrenTotalWidth) {
        var currentPosition = GetAnchorPositionByAlignment(childrenTotalWidth);
        foreach (CardWrapper child in cards) {
            var adjustedChildWidth = child.width * child.transform.lossyScale.x;
            child.targetPosition = new Vector2(currentPosition + adjustedChildWidth / 2, transform.position.y);
            currentPosition += adjustedChildWidth;
        }
    }

    private float GetAnchorPositionByAlignment(float childrenWidth) {
        var containerWidthInGlobalSpace = rectTransform.rect.width * transform.lossyScale.x;
        switch (alignment) {
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

    private void SetCardsAnchor() {
        foreach (CardWrapper child in cards) {
            child.SetAnchor(new Vector2(0, 0.5f), new Vector2(0, 0.5f));
        }
    }

    public void OnCardDragStart(CardWrapper card) {
        currentDraggedCard = card;
    }

    public void OnCardDragEnd(float Ydis) {
        if (!combatStateMachine._isPlayState || currentDraggedCard == null)
            return;

        foreach (GameObject playAreaObject in cardPlayConfig.playArea)
        {
            // If card is in play area, play it!
            if (IsCursorInPlayArea(playAreaObject.GetComponent<RectTransform>()))
            {
                if (Ydis < YAmount) return;

                eventsConfig?.OnCardPlayed?.Invoke(new CardPlayed(currentDraggedCard), currentDraggedCard.card, playAreaObject.transform.tag);
                if (cardPlayConfig.destroyOnPlay)
                {
                    DestroyCard(currentDraggedCard);
                }
            }
        }

        currentDraggedCard = null;
    }
    
    public void DestroyCard(CardWrapper card) {
        cards.Remove(card);
        eventsConfig.OnCardDestroy?.Invoke(new CardDestroy(card));
        Destroy(card.gameObject);
    }

    private bool IsCursorInPlayArea(RectTransform playArea) {
        if (cardPlayConfig.playArea == null) return false;
        
        var cursorPosition = Input.mousePosition;
        var playAreaCorners = new Vector3[4];
        playArea.GetWorldCorners(playAreaCorners);
        return (cursorPosition.x > playAreaCorners[0].x &&
               cursorPosition.x < playAreaCorners[2].x &&
               cursorPosition.y > playAreaCorners[0].y &&
               cursorPosition.y < playAreaCorners[2].y);
    }
}
