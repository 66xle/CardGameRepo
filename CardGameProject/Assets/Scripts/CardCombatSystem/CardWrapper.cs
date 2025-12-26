using config;
using events;
using MyBox;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardWrapper : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerMoveHandler,
    IPointerUpHandler {
    private const float EPS = 0.01f;

    public CombatStateMachine combatStateMachine;
    public Canvas mainCanvas;
    public Card card;
    public float targetRotation;
    public Vector2 targetPosition;
    public float targetVerticalDisplacement;
    public int uiLayer;
    public Vector2 targetScale;

    private RectTransform rectTransform;
    [HideInInspector] public Canvas canvas;

    public ZoomConfig zoomConfig;
    public AnimationSpeedConfig animationSpeedConfig;
    public CardContainer container;

    private bool isHovered;
    private bool isDragged = false;
    public EventsConfig eventsConfig;

    private float dragStartEventY;
    private float pointDownStartEventY;

    private bool isPointerDown = false;
    private bool pointerDownCheck = false;
    [HideInInspector] public bool IsPreviewActive = false;

    public float width {
        get => rectTransform.rect.width;
    }

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start() {
        canvas = GetComponent<Canvas>();
    }

    private void Update() {

        if (Time.timeScale == 0) return;

        UpdateRotation();
        UpdatePosition();
        UpdateScale();
        UpdateUILayer();


        if (!InputManager.Instance.LeftClickInputDown && isDragged && !pointerDownCheck)
        {
            PointerUp(Input.mousePosition);
        }
    }

    private void UpdateUILayer() {
        if (!IsPreviewActive && !isDragged) {
            canvas.sortingOrder = uiLayer;
        }
    }

    private void UpdatePosition()
    {
        if (!isDragged)
        {
            var target = new Vector2(targetPosition.x, targetPosition.y + targetVerticalDisplacement);
            if (IsPreviewActive && zoomConfig.overrideYPosition != -1)
            {
                target = new Vector2(target.x, zoomConfig.overrideYPosition);
            }

            var distance = Vector2.Distance(rectTransform.localPosition, target);
            var repositionSpeed = distance / animationSpeedConfig.duration;

            if (repositionSpeed == 0)
                repositionSpeed = 1;

            Vector2 position = Vector2.Lerp(rectTransform.localPosition, target, repositionSpeed / distance * Time.deltaTime);

            if (float.IsNaN(position.x) || float.IsNaN(position.y)) return;

            rectTransform.localPosition = position;
        }
        else
        {
            // Works for both PC (mouse) and mobile (touch)
            Vector2 pointerPos = (Input.touchCount > 0) ? Input.GetTouch(0).position : (Vector2)Input.mousePosition;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            container.mainCanvas.transform as RectTransform,
            pointerPos,
            container.mainCanvas.worldCamera,
            out Vector2 localPoint))
            {
                rectTransform.localPosition = localPoint + new Vector2(0, rectTransform.rect.height * 0.7f);
            }
        }
    }

    private void UpdateScale() {
        var targetZoom = (isDragged || IsPreviewActive) && zoomConfig.zoomOnClick ? zoomConfig.multiplier : 1;
        if (container.currentInPlayArea)
            targetZoom = zoomConfig.zoomInMultiplier;


        var delta = Mathf.Abs(rectTransform.localScale.x - targetZoom);
        float newZoom = Mathf.Lerp(rectTransform.localScale.x, targetZoom,
            animationSpeedConfig.zoom / delta * Time.deltaTime);

        if (float.IsNaN(newZoom)) return;

        rectTransform.localScale = new Vector3(newZoom, newZoom, 1);
    }

    private void UpdateRotation()
    {
        // Determine target rotation (0 if zoom reset)
        float tempTargetRotation = (IsPreviewActive || isDragged) && zoomConfig.resetRotationOnZoom
            ? 0
            : targetRotation;

        float currentAngle = rectTransform.localEulerAngles.z;

        // Calculate the shortest signed difference (-180, 180)
        float delta = Mathf.DeltaAngle(currentAngle, tempTargetRotation);

        // If we're close enough, snap to final rotation
        if (Mathf.Abs(delta) <= EPS)
        {
            rectTransform.localRotation = Quaternion.Euler(0, 0, tempTargetRotation);
            return;
        }

        // Smoothly rotate towards target
        float nextAngle = Mathf.MoveTowardsAngle(currentAngle, tempTargetRotation,
            animationSpeedConfig.rotation * Time.deltaTime);

        rectTransform.localRotation = Quaternion.Euler(0, 0, nextAngle);
    }


    public void SetAnchor(Vector2 min, Vector2 max) {
        rectTransform.anchorMin = min;
        rectTransform.anchorMax = max;
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (isPointerDown && !isDragged)
        {
            // Convert both positions to local space
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                container.mainCanvas.transform as RectTransform,
                eventData.position,
                container.mainCanvas.worldCamera,
                out Vector2 currentLocal);

            //float Ydis = currentLocal.y - pointDownStartEventY;

            //if (Ydis < 10f) return;

            isDragged = true;
            dragStartEventY = currentLocal.y;

            container.OnCardDragStart(this);
            eventsConfig?.OnCardDrag?.Invoke(new CardDrag(this));
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (isDragged) return;

        Debug.Log("enter");

        if (InputManager.Instance.HoldLeftClickInput && !isPointerDown && container.currentDraggedCard == null)
        {
            isPointerDown = true;
            pointDownStartEventY = Input.mousePosition.y;
            eventsConfig?.OnCardClick?.Invoke(new CardClick(this));
            return;
        }

        eventsConfig?.OnCardHover?.Invoke(new CardHover(this));
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (isDragged) {
            // Avoid hover events while dragging
            return;
        }

        if (isPointerDown)
        {
            isPointerDown = false;
        }

        isHovered = false;
        eventsConfig?.OnCardUnhover?.Invoke(new CardUnhover(this));
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (!combatStateMachine._isPlayState)
            return;

        isPointerDown = true;
        pointerDownCheck = true;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
        container.mainCanvas.transform as RectTransform,
        eventData.position,
        container.mainCanvas.worldCamera,
        out Vector2 localPoint);

        pointDownStartEventY = localPoint.y;

        eventsConfig?.OnCardClick?.Invoke(new CardClick(this));
    }

    public void OnPointerUp(PointerEventData eventData) {
        
        PointerUp(eventData.position);
    }

    public void PointerUp(Vector2 position)
    {
        isPointerDown = false;
        pointerDownCheck = false;
        isDragged = false;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
        container.mainCanvas.transform as RectTransform,
        position,
        container.mainCanvas.worldCamera,
        out Vector2 localPoint);


        float Ydis = localPoint.y - dragStartEventY;
        container.OnCardDragEnd(Ydis);
    }
}
