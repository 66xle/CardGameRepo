using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkipCutscene : MonoBehaviour
{
    [Header("UI References")]
    public CanvasGroup skipButton;

    [Header("Settings")]
    public float visibleDuration = 3f;      // how long button stays visible after last input
    public float fadeDuration = 0.5f;       // fade-out time

    private Coroutine fadeCoroutine;
    private float lastInteractionTime;
    private bool isVisible;

    void Start()
    {
        if (skipButton != null)
        {
            skipButton.alpha = 0f;
            skipButton.interactable = false;
            skipButton.blocksRaycasts = false;
        }
    }

    void Update()
    {
        // Detect any interaction: key, mouse click, or screen tap
        if (Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.touchCount > 0)
        {
            ShowSkipButton();
        }

        // Hide after timeout
        if (isVisible && Time.time - lastInteractionTime > visibleDuration)
        {
            HideSkipButton();
        }
    }

    public void ShowSkipButton()
    {
        lastInteractionTime = Time.time;

        if (!isVisible)
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeCanvasGroup(skipButton, skipButton.alpha, 1f, fadeDuration));
            skipButton.interactable = true;
            skipButton.blocksRaycasts = true;
            isVisible = true;
        }
    }

    public void HideSkipButton()
    {
        if (isVisible)
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeCanvasGroup(skipButton, skipButton.alpha, 0f, fadeDuration));
            skipButton.interactable = false;
            skipButton.blocksRaycasts = false;
            isVisible = false;
        }
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            cg.alpha = Mathf.Lerp(start, end, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        cg.alpha = end;
    }
}
