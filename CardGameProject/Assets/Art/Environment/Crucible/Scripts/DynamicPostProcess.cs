using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CloudlightTintLooper : MonoBehaviour
{
    [Header("Volume Settings")]
    public VolumeProfile volumeProfile; // Reference the actual profile here

    [Header("Color Anchors (min 2, max 3 recommended)")]
    public Color[] shadowsColors;
    public Color[] midtonesColors;
    public Color[] highlightsColors;

    [Header("Loop Settings")]
    public float minDuration = 8f;
    public float maxDuration = 15f;
    public bool useSinusoidal = true;

    private ShadowsMidtonesHighlights smh;
    private int currentIndex = 0;
    private int nextIndex = 1;
    private float t = 0f;
    private float currentDuration;

    private void Start()
    {
        if (volumeProfile == null)
        {
            Debug.LogError("No VolumeProfile assigned.");
            enabled = false;
            return;
        }

        if (!volumeProfile.TryGet(out smh))
        {
            Debug.LogError("ShadowsMidtonesHighlights override not found in VolumeProfile.");
            enabled = false;
            return;
        }

        if (shadowsColors.Length < 2 || midtonesColors.Length < 2 || highlightsColors.Length < 2)
        {
            Debug.LogError("Please assign at least 2 colors to each band.");
            enabled = false;
            return;
        }

        EnableSMH();
        PickNextTarget();
    }

    private void Update()
    {
        if (smh == null) return;

        t += Time.deltaTime / currentDuration;
        float blendT = useSinusoidal ? (0.5f - 0.5f * Mathf.Cos(t * Mathf.PI)) : Mathf.SmoothStep(0f, 1f, t);

        smh.shadows.value = Color.Lerp(shadowsColors[currentIndex], shadowsColors[nextIndex], blendT);
        smh.midtones.value = Color.Lerp(midtonesColors[currentIndex], midtonesColors[nextIndex], blendT);
        smh.highlights.value = Color.Lerp(highlightsColors[currentIndex], highlightsColors[nextIndex], blendT);

        if (t >= 1f)
        {
            currentIndex = nextIndex;
            PickNextTarget();
        }
    }

    private void PickNextTarget()
    {
        t = 0f;
        currentDuration = Random.Range(minDuration, maxDuration);
        nextIndex = (currentIndex + 1) % shadowsColors.Length;
    }

    private void EnableSMH()
    {
        smh.active = true;
        smh.shadows.overrideState = true;
        smh.midtones.overrideState = true;
        smh.highlights.overrideState = true;
    }
}