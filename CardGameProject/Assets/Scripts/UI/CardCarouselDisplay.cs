using config;
using events;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class CardCarouselDisplay : MonoBehaviour, IPointerDownHandler, IPointerMoveHandler,
    IPointerUpHandler {
    private const float EPS = 0.01f;

    public Card card;
    public float targetRotation;
    public Vector2 targetPosition;
    public float targetVerticalDisplacement;
    public int uiLayer;

    private RectTransform rectTransform;
    [HideInInspector] public Canvas canvas;

    public ZoomConfig zoomConfig;
    public AnimationSpeedConfig animationSpeedConfig;
    public CardCarousel container;

    private Vector2 dragStartPos;
    public EventsConfig eventsConfig;
    public Vector2 basePosition;

    private bool isPointerDown = false;
    private bool isDragging = false;
    private float pointDownStartEventX;
    [HideInInspector] public bool IsPreviewActive = false;

    public float width
    {
        get => rectTransform.rect.width;
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        canvas = GetComponent<Canvas>();
    }

    private void Update()
    {
        UpdatePosition();
        UpdateScale();
    }


    private void UpdatePosition()
    {
        if (!IsPreviewActive)
        {
            var target = new Vector2(targetPosition.x, targetPosition.y + targetVerticalDisplacement);
            if (IsPreviewActive && zoomConfig.overrideYPosition != -1)
            {
                target = new Vector2(target.x, zoomConfig.overrideYPosition);
            }

            var distance = Vector2.Distance(rectTransform.position, target);
            var repositionSpeed = distance / animationSpeedConfig.duration;

            if (repositionSpeed == 0)
                repositionSpeed = 1;

            Vector2 position = Vector2.Lerp(rectTransform.position, target, repositionSpeed / distance * Time.unscaledDeltaTime);

            if (float.IsNaN(position.x) || float.IsNaN(position.y)) return;

            rectTransform.position = position;
        }
        else
        {
            //var delta = ((Vector2)Input.mousePosition + dragStartPos);
            //rectTransform.position = new Vector2(delta.x, delta.y);
        }
    }

    private void UpdateScale()
    {
        var targetZoom = (IsPreviewActive) && zoomConfig.zoomOnClick ? zoomConfig.multiplier : 1;
        var delta = Mathf.Abs(rectTransform.localScale.x - targetZoom);
        float newZoom = Mathf.Lerp(rectTransform.localScale.x, targetZoom,
            animationSpeedConfig.zoom / delta * Time.unscaledDeltaTime);

        if (float.IsNaN(newZoom)) return;

        rectTransform.localScale = new Vector3(newZoom, newZoom, 1);
    }
    public void SetAnchor(Vector2 min, Vector2 max)
    {
        rectTransform.anchorMin = min;
        rectTransform.anchorMax = max;
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (isPointerDown && !isDragging)
        {
            float XDis = eventData.position.x - pointDownStartEventX;

            if (Mathf.Abs(XDis) > 0)
            {
                Vector2 dragStartPos = eventData.position;

                isDragging = true;
                container.OnCardDragStart(this, dragStartPos);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPointerDown = true;
        pointDownStartEventX = eventData.position.x;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPointerDown = false;

        if (!isDragging && !IsPreviewActive)
        {
            container.OnClickStart(this);
        }

        if (isDragging)
        {
            isDragging = false;
            container.OnCardDragEnd();
        }
    }
}
