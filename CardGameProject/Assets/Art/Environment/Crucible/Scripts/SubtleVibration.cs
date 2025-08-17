using UnityEngine;

/// Add to any Transform (elevator mesh root or camera). Subtle, varying micro-motion.
/// Mobile-friendly: no allocations, Perlin noise only, toggleable at runtime.
[DisallowMultipleComponent]
public class SubtleVibration : MonoBehaviour
{
    [Header("Position")]
    [Tooltip("Max local position offset in meters (x,y,z).")]
    public Vector3 posAmplitude = new Vector3(0.003f, 0.0015f, 0.003f);

    [Tooltip("Noise frequency (cycles per second). Lower = slower drift.")]
    [Range(0.05f, 5f)] public float posFrequency = 0.6f;

    [Header("Rotation")]
    [Tooltip("Max local rotation offset in degrees (x,y,z).")]
    public Vector3 rotAmplitude = new Vector3(0.2f, 0.05f, 0.2f);

    [Tooltip("Noise frequency for rotation.")]
    [Range(0.05f, 5f)] public float rotFrequency = 0.9f;

    [Header("Feel")]
    [Tooltip("Overall intensity multiplier (0..1+).")]
    [Range(0f, 2f)] public float intensity = 1f;

    [Tooltip("Random seed for decorrelation between instances.")]
    public int seed = 12345;

    [Tooltip("Update in LateUpdate to play nice with other movers/animators.")]
    public bool updateInLateUpdate = true;

    Transform _t;
    Vector3 _baseLocalPos;
    Quaternion _baseLocalRot;
    Vector3 _pOffset, _rOffset; // per-instance noise offsets

    void OnEnable()
    {
        _t = transform;
        _baseLocalPos = _t.localPosition;
        _baseLocalRot = _t.localRotation;

        // Derive stable offsets from seed (keeps noise de-correlated per axis)
        var s = Mathf.Abs(seed) + 1;
        _pOffset = new Vector3(Hash01(s * 11), Hash01(s * 23), Hash01(s * 37)) * 10f;
        _rOffset = new Vector3(Hash01(s * 41), Hash01(s * 59), Hash01(s * 71)) * 10f;
    }

    void OnDisable()
    {
        if (_t == null) return;
        _t.localPosition = _baseLocalPos;
        _t.localRotation = _baseLocalRot;
    }

    void Update()
    {
        if (!updateInLateUpdate) Apply();
    }

    void LateUpdate()
    {
        if (updateInLateUpdate) Apply();
    }

    void Apply()
    {
        float t = Time.time;

        // Position noise: centered around 0 by mapping [0,1] -> [-1,1]
        Vector3 p = new Vector3(
            (Mathf.PerlinNoise(_pOffset.x, t * posFrequency) - 0.5f) * 2f * posAmplitude.x,
            (Mathf.PerlinNoise(_pOffset.y, t * (posFrequency * 1.11f)) - 0.5f) * 2f * posAmplitude.y,
            (Mathf.PerlinNoise(_pOffset.z, t * (posFrequency * 0.87f)) - 0.5f) * 2f * posAmplitude.z
        ) * intensity;

        // Rotation noise (degrees)
        Vector3 r = new Vector3(
            (Mathf.PerlinNoise(_rOffset.x, t * rotFrequency) - 0.5f) * 2f * rotAmplitude.x,
            (Mathf.PerlinNoise(_rOffset.y, t * (rotFrequency * 0.92f)) - 0.5f) * 2f * rotAmplitude.y,
            (Mathf.PerlinNoise(_rOffset.z, t * (rotFrequency * 1.19f)) - 0.5f) * 2f * rotAmplitude.z
        ) * intensity;

        _t.localPosition = _baseLocalPos + p;
        _t.localRotation = _baseLocalRot * Quaternion.Euler(r);
    }

    // Tiny, deterministic 0..1 hash from int
    static float Hash01(int n)
    {
        unchecked
        {
            n = (n << 13) ^ n;
            return (1f - ((n * (n * n * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824f);
        }
    }
}