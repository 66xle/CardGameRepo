using UnityEngine;

[CreateAssetMenu(fileName = "TunnelSection", menuName = "EndlessTunnel/Section", order = 0)]
public class TunnelSectionSO : ScriptableObject
{
    [Header("Prefab")]
    [Tooltip("The prefab to spawn for this section.")]
    public GameObject prefab;

    [Header("Height")]
    [Tooltip("Default stacking height (world units) for this section.")]
    public float baseHeight = 10f;

    [Tooltip("If true, use a custom stacking height (half-height, tall piece, etc.).")]
    public bool useCustomHeight = false;

    [Tooltip("Overrides baseHeight if enabled.")]
    public float customHeight = 10f;

    [Header("Rotation")]
    [Tooltip("Snap rotation step in degrees (e.g., 60 for hex-symmetry).")]
    public float rotationStepDegrees = 60f;

    [Tooltip("Locked rotation (in degrees) used only for Cutscene mode.")]
    public float initialRotation = 0f;

    [Header("Selection")]
    [Range(0f, 1f)]
    [Tooltip("Relative chance to spawn (weighted random). 0 disables.")]
    public float spawnWeight = 1f;

    [Tooltip("Minimum progress needed for this to appear.")]
    public int minProgressLevel = 0;

    [Header("Helical (Optional)")]
    [Tooltip("If true, this piece may apply a helical offset when spawned.")]
    public bool canApplyHelix = false;

    [Range(0f, 1f)]
    [Tooltip("Chance this piece applies helix on spawn.")]
    public float helixChance = 0.25f;

    [Tooltip("Radius range for the helix offset in local XZ.")]
    public Vector2 helixRadiusRange = new Vector2(0.5f, 1.5f);

    [Tooltip("Degrees to advance the spiral when this piece applies helix (per piece).")]
    public Vector2 helixDegreesPerSectionRange = new Vector2(10f, 25f);

    [Tooltip("Random phase jitter in degrees (applied once per spawned piece).")]
    public Vector2 helixPhaseJitterRange = new Vector2(-15f, 15f);

    [Header("Arena Settings")]
    [Tooltip("Local Y offset (from base) where the tower should stop when this section is used as an arena.")]
    public float stopOffset = 0f;

    /// <summary>
    /// Returns either the baseHeight or customHeight, depending on settings.
    /// Always clamps to at least 0.01 to avoid zero-size errors.
    /// </summary>
    public float EffectiveHeight
    {
        get { return useCustomHeight ? Mathf.Max(0.01f, customHeight) : Mathf.Max(0.01f, baseHeight); }
    }
}