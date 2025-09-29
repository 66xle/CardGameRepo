using config;
using events;
using MyBox;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardWrapper : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerMoveHandler,
    IPointerUpHandler {
    private const float EPS = 0.01f;

    public CombatStateMachine combatStateMachine;
    public Card card;
    public float targetRotation;
    public Vector2 targetPosition;
    public float targetVerticalDisplacement;
    public int uiLayer;

    private RectTransform rectTransform;
    [HideInInspector] public Canvas canvas;

    public ZoomConfig zoomConfig;
    public AnimationSpeedConfig animationSpeedConfig;
    public CardContainer container;

    private bool isHovered;
    private bool isDragged = false;
    private Vector2 dragStartPos;
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
            var delta = ((Vector2)Input.mousePosition + dragStartPos);
            rectTransform.localPosition = new Vector2(delta.x, delta.y);
        }
    }

    private void UpdateScale() {
        var targetZoom = (isDragged || IsPreviewActive) && zoomConfig.zoomOnClick ? zoomConfig.multiplier : 1;
        var delta = Mathf.Abs(rectTransform.localScale.x - targetZoom);
        float newZoom = Mathf.Lerp(rectTransform.localScale.x, targetZoom,
            animationSpeedConfig.zoom / delta * Time.deltaTime);

        if (float.IsNaN(newZoom)) return;

        rectTransform.localScale = new Vector3(newZoom, newZoom, 1);
    }

    //private void UpdateRotation() {
    //    // If the card is hovered and the rotation should be reset, set the target rotation to 0
    //    var tempTargetRotation = (IsPreviewActive || isDragged) && zoomConfig.resetRotationOnZoom
    //        ? 0
    //        : targetRotation;

    //    var crtAngle = rectTransform.rotation.eulerAngles.z;
    //    // If the angle is negative, add 360 to it to get the positive equivalent
    //    crtAngle = crtAngle < 0 ? crtAngle + 360 : crtAngle;
    //    tempTargetRotation = tempTargetRotation < 0 ? tempTargetRotation + 360 : tempTargetRotation;
        
    //    var deltaAngle = Mathf.Abs(crtAngle - tempTargetRotation);
    //    Debug.Log(targetRotation + " | " + deltaAngle);
    //    if (!(deltaAngle > EPS)) return;



    //    // Adjust the current angle and target angle so that the rotation is done in the shortest direction
    //    var adjustedCurrent = deltaAngle > 180 && crtAngle < tempTargetRotation ? crtAngle + 360 : crtAngle;
    //    var adjustedTarget = deltaAngle > 180 && crtAngle > tempTargetRotation ? tempTargetRotation + 360 : tempTargetRotation;

    //    var newDelta = Mathf.Abs(adjustedCurrent - adjustedTarget);
    //    var nextRotation = Mathf.Lerp(adjustedCurrent, adjustedTarget, animationSpeedConfig.rotation / newDelta * Time.deltaTime);

    //    rectTransform.localRotation = Quaternion.Euler(0, 0, nextRotation);
    //}

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
            float Ydis = Input.mousePosition.y - pointDownStartEventY;

            if (Ydis < 50f) return;

            isDragged = true;
            dragStartPos = new Vector2(transform.localPosition.x - eventData.position.x,
                transform.localPosition.y - eventData.position.y);

            dragStartEventY = Input.mousePosition.y;

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
        pointDownStartEventY = eventData.position.y;

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
        float Ydis = position.y - dragStartEventY;
        container.OnCardDragEnd(Ydis);
    }
}
