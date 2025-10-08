using config;
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
    [MustBeAssigned] public Canvas mainCanvas;
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
            wrapper.mainCanvas = mainCanvas;

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

        SortCards();
        SetCardsPosition();
        SetCardsRotation();
        SetCardsUILayers();
        UpdateCardOrder();
    }

    private void SortCards()
    {
        cards.Sort((a, b) => string.Compare(a.card.CardName, b.card.CardName));
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
        var cardsTotalWidth = cards.Sum(card => card.width);
        // Compute the width of the container in global space
        var containerWidth = rectTransform.rect.width ;
        if (forceFitContainer && cardsTotalWidth > containerWidth) {
            DistributeChildrenToFitContainer(cardsTotalWidth);
        }
        else {
            DistributeChildrenWithoutOverlap(cardsTotalWidth);
        }
    }

    //private void DistributeChildrenToFitContainer(float childrenTotalWidth)
    //{

    //    int selectedIndex = clonePreviewManager.currentCard == null ? -1 : cards.IndexOf(clonePreviewManager.currentCard);

    //    // Get the width of the container
    //    var width = rectTransform.rect.width * transform.lossyScale.x;
    //    // Get the distance between each child
    //    var distanceBetweenChildren = (width - childrenTotalWidth) / (cards.Count - 1);
    //    // Set all children's positions to be evenly spaced out
    //    var currentX = transform.localPosition.x - width / 2;

    //    for (int i = 0; i < cards.Count; i++)
    //    {
    //        var child = cards[i];
    //        float adjustedChildWidth = child.width * child.transform.lossyScale.x;

    //        // Base X without push
    //        float baseX = currentX + adjustedChildWidth / 2;

    //        // Apply push if any card is selected
    //        float pushOffset = 0f;
    //        if (selectedIndex >= 0 && cards.Count >= pushAffectedCardCount)
    //        {
    //            int distanceFromSelected = Mathf.Abs(i - selectedIndex);

    //            if (distanceFromSelected <= affectedCardCount && i != selectedIndex)
    //            {
    //                // Normalized falloff: closer cards are pushed more
    //                float t = 1f - (distanceFromSelected / (float)(affectedCardCount + 1));
    //                t = Mathf.Pow(t, falloff);

    //                // Right side bias: push cards on the right harder
    //                if (i > selectedIndex)
    //                {
    //                    t *= rightPushMultiplier; // e.g., 1.25f
    //                }

    //                float direction = Mathf.Sign(i - selectedIndex); // -1 = left, +1 = right
    //                pushOffset = direction * pushAmount * t;
    //            }
    //        }

    //        child.targetPosition = new Vector2(baseX + pushOffset, transform.localPosition.y);
    //        currentX += adjustedChildWidth + distanceBetweenChildren;
    //    }
    //}


    private void DistributeChildrenToFitContainer(float childrenTotalWidth)
    {
        int selectedIndex = clonePreviewManager.currentCard == null ? -1 : cards.IndexOf(clonePreviewManager.currentCard);

        // Container width in local space
        float width = rectTransform.rect.width;
        // Distance between children in local space
        float distanceBetweenChildren = (width - childrenTotalWidth) / (cards.Count - 1);
        // Start at left edge in local space
        float currentX = -width / 2f;

        for (int i = 0; i < cards.Count; i++)
        {
            var child = cards[i];
            float adjustedChildWidth = child.width; // Already in local space (no lossyScale needed here)

            // Base X (relative to container)
            float baseX = currentX + adjustedChildWidth / 2f;

            // Push offset in local space
            float pushOffset = 0f;
            if (selectedIndex >= 0 && cards.Count >= pushAffectedCardCount)
            {
                int distanceFromSelected = Mathf.Abs(i - selectedIndex);
                if (distanceFromSelected <= affectedCardCount && i != selectedIndex)
                {
                    float t = 1f - (distanceFromSelected / (float)(affectedCardCount + 1));
                    t = Mathf.Pow(t, falloff);

                    if (i > selectedIndex)
                        t *= rightPushMultiplier;

                    float direction = Mathf.Sign(i - selectedIndex);
                    pushOffset = direction * pushAmount * t;
                }
            }

            // Apply local position relative to container
            child.targetPosition = new Vector2(baseX + pushOffset, 0f);
            currentX += adjustedChildWidth + distanceBetweenChildren;
        }
    }


    //private void DistributeChildrenWithoutOverlap(float childrenTotalWidth) {
    //    var currentPosition = GetAnchorPositionByAlignment(childrenTotalWidth);
    //    foreach (CardWrapper child in cards) {
    //        var adjustedChildWidth = child.width * child.transform.lossyScale.x;
    //        child.targetPosition = new Vector2(currentPosition + adjustedChildWidth / 2, transform.localPosition.y);
    //        currentPosition += adjustedChildWidth;
    //    }
    //}

    private void DistributeChildrenWithoutOverlap(float childrenTotalWidth)
    {
        float startX = GetAnchorPositionByAlignment(childrenTotalWidth);

        foreach (CardWrapper child in cards)
        {
            float adjustedChildWidth = child.width; // width is local
            float baseX = startX + adjustedChildWidth / 2f;

            // Position relative to container
            child.targetPosition = new Vector2(baseX, 0f);

            startX += adjustedChildWidth;
        }
    }

    //private float GetAnchorPositionByAlignment(float childrenWidth)
    //{
    //    var containerWidthInGlobalSpace = rectTransform.rect.width * transform.lossyScale.x;
    //    switch (alignment)
    //    {
    //        case CardAlignment.Left:
    //            return transform.localPosition.x - containerWidthInGlobalSpace / 2;
    //        case CardAlignment.Center:
    //            return transform.localPosition.x - childrenWidth / 2;
    //        case CardAlignment.Right:
    //            return transform.localPosition.x + containerWidthInGlobalSpace / 2 - childrenWidth;
    //        default:
    //            return 0;
    //    }
    //}

    private float GetAnchorPositionByAlignment(float childrenWidth)
    {
        float containerWidth = rectTransform.rect.width;
        switch (alignment)
        {
            case CardAlignment.Left:
                return -containerWidth / 2f;
            case CardAlignment.Center:
                return -childrenWidth / 2f;
            case CardAlignment.Right:
                return containerWidth / 2f - childrenWidth;
            default:
                return 0f;
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
            if (playAreaObject == null) continue;

            // If card is in play area, play it!
            RectTransform rectTransform = playAreaObject.GetComponent<RectTransform>();
            if (IsCursorInPlayArea(rectTransform))
            {
                if (playAreaObject.tag == "Play" && Ydis < YAmount) return;

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

    public void DestroyAllCards()
    {
        for (int i = cards.Count - 1; i >= 0; i--)
        {
            DestroyCard(cards[i]);
        }
    }

    private bool IsCursorInPlayArea(RectTransform playArea) {
        if (cardPlayConfig.playArea == null) return false;
        
        var cursorPosition = Input.mousePosition;
        var playAreaCorners = new Vector3[4];
        playArea.GetWorldCorners(playAreaCorners);

        // Convert world corners to screen space using the camera
        Vector3 bottomLeft = Camera.main.WorldToScreenPoint(playAreaCorners[0]);
        Vector3 topRight = Camera.main.WorldToScreenPoint(playAreaCorners[2]);

        return (cursorPosition.x > bottomLeft.x &&
               cursorPosition.x < topRight.x &&
               cursorPosition.y > bottomLeft.y &&
               cursorPosition.y < topRight.y);
    }
}
