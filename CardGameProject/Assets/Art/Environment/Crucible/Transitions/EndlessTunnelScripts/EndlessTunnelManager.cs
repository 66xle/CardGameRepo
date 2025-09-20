using System.Collections.Generic;
using Systems.SceneManagment;
using UnityEngine;

public class EndlessTunnelManager : MonoBehaviour
{
    [Header("Setup")]
    [Tooltip("Parent under which all spawned sections live and are scrolled.")]
    public Transform tunnelParent;

    [Tooltip("ScriptableObject library of available sections.")]
    public List<TunnelSectionSO> sectionLibrary = new List<TunnelSectionSO>();

    [Tooltip("How many sections to spawn at start.")]
    public int initialSections = 6;

    [Header("Spawn/Despawn Window (World Space)")]
    [Tooltip("Ensure content exists up to this world Y above the parent (top horizon).")]
    public float spawnWindowAhead = 80f;

    [Tooltip("Despawns sections once their TOP edge passes below this world Y.")]
    public float despawnBelow = -80f;

    [Header("Runtime Control")]
    [Tooltip("Run immediately on Start.")]
    public bool runAtStart = true;

    [Tooltip("Gates which sections are eligible (minProgressLevel <= currentProgressLevel).")]
    public int currentProgressLevel = 0;

    [Header("Speed Drift")]
    [Tooltip("Min/Max scroll speed (world units per second).")]
    public Vector2 speedRange = new Vector2(1.5f, 3.0f);

    [Tooltip("Min/Max seconds BETWEEN the start of drift events (idle gap).")]
    public Vector2 driftIdleIntervalRange = new Vector2(6f, 14f);

    [Tooltip("Min/Max seconds to ramp between current and target speed.")]
    public Vector2 driftRampDurationRange = new Vector2(5f, 10f);

    // Internal state
    private struct Spawned
    {
        public GameObject go;
        public float localY; // base Y (local to parent)
        public float height; // effective height
    }

    private readonly List<Spawned> _active = new List<Spawned>();
    private float _nextStackY = 0f;     // local Y cursor for next base
    private bool _running;

    // Speed drift
    private float _currentSpeed;
    private float _targetSpeed;
    private float _driftT;              // 0..1 ramp progress
    private float _driftDuration;       // seconds
    private float _driftIdleTimer;      // seconds until next drift
    private bool  _drifting;

    // Helix
    private float _helixPhaseAccum = 0f; // degrees, accumulates when helix is applied

    // cache to avoid allocs
    private static readonly List<TunnelSectionSO> _cacheEligible = new List<TunnelSectionSO>();

    private SceneLoader SceneLoader;

    private void Awake()
    {
        if (!runAtStart)
            SceneInitialize.Instance.Subscribe(Init);
    }

    void Start()
    {
        if (tunnelParent == null) tunnelParent = transform;

        _running = runAtStart;

        // Initialize drift
        float mid = (speedRange.x + speedRange.y) * 0.5f;
        _currentSpeed = Mathf.Clamp(mid, speedRange.x, speedRange.y);
        _targetSpeed  = _currentSpeed;
        _driftIdleTimer = Random.Range(driftIdleIntervalRange.x, driftIdleIntervalRange.y);

        ResetAndPrewarm();
        MaintainSpawnWindow();
    }

    public void Init()
    {
        _running = true;
    }

    void Update()
    {
        if (!_running) return;

        UpdateSpeedDrift();
        tunnelParent.position += Vector3.down * (_currentSpeed * Time.deltaTime);

        MaintainSpawnWindow();
        DespawnPassed();
    }

    // ---------- Public API ----------

    public void SetRunning(bool on) { _running = on; }

    public void SetProgress(int level) { currentProgressLevel = level; }

    public void ResetAndPrewarm()
    {
        for (int i = _active.Count - 1; i >= 0; i--)
        {
            if (_active[i].go) Destroy(_active[i].go);
        }
        _active.Clear();

        _nextStackY = 0f;
        _helixPhaseAccum = 0f;

        for (int i = 0; i < initialSections; i++)
            SpawnNext();
    }

    /// <summary>
    /// World-space Y where the next section base would spawn (useful for transitions).
    /// </summary>
    public float GetNextBaseWorldY()
    {
        return tunnelParent.TransformPoint(new Vector3(0f, _nextStackY, 0f)).y;
    }

    // ---------- Core Loop ----------

    private void MaintainSpawnWindow()
    {
        float topWorldY = GetNextBaseWorldY();

        int safety = 0;
        while (topWorldY < spawnWindowAhead && safety++ < 100)
        {
            SpawnNext();
            topWorldY = GetNextBaseWorldY();
        }
    }

    private void DespawnPassed()
    {
        for (int i = _active.Count - 1; i >= 0; i--)
        {
            Spawned s = _active[i];
            float topLocalY = s.localY + s.height;
            float topWorldY = tunnelParent.TransformPoint(new Vector3(0f, topLocalY, 0f)).y;

            if (topWorldY < despawnBelow)
            {
                if (s.go) Destroy(s.go);
                _active.RemoveAt(i);
            }
        }
    }

    private void SpawnNext()
    {
        TunnelSectionSO def = PickDefinition();
        if (def == null || def.prefab == null) return;

        GameObject go = Instantiate(def.prefab, tunnelParent);

        // Base local position
        Vector3 localPos = new Vector3(0f, _nextStackY, 0f);

        // Optional helix offset
        if (def.canApplyHelix && Random.value <= def.helixChance)
        {
            float radius  = Random.Range(def.helixRadiusRange.x, def.helixRadiusRange.y);
            float advance = Random.Range(def.helixDegreesPerSectionRange.x, def.helixDegreesPerSectionRange.y);
            float jitter  = Random.Range(def.helixPhaseJitterRange.x, def.helixPhaseJitterRange.y);

            _helixPhaseAccum += advance;
            float phaseRad = (_helixPhaseAccum + jitter) * Mathf.Deg2Rad;
            localPos.x += Mathf.Cos(phaseRad) * radius;
            localPos.z += Mathf.Sin(phaseRad) * radius;
        }

        go.transform.localPosition = localPos;

        // Rotation snap (e.g., 60-degree steps)
        float step = Mathf.Max(1f, def.rotationStepDegrees);
        int steps = Mathf.Max(1, Mathf.RoundToInt(360f / step));
        int r = Random.Range(0, steps);
        go.transform.localRotation = Quaternion.Euler(0f, r * step, 0f);

        float h = def.EffectiveHeight;
        Spawned spawned = new Spawned();
        spawned.go = go;
        spawned.localY = _nextStackY;
        spawned.height = h;
        _active.Add(spawned);

        _nextStackY += h;
    }

    private TunnelSectionSO PickDefinition()
    {
        _cacheEligible.Clear();
        float total = 0f;

        for (int i = 0; i < sectionLibrary.Count; i++)
        {
            TunnelSectionSO d = sectionLibrary[i];
            if (d == null || d.prefab == null) continue;
            if (d.minProgressLevel > currentProgressLevel) continue;
            if (d.spawnWeight <= 0f) continue;

            _cacheEligible.Add(d);
            total += d.spawnWeight;
        }

        if (_cacheEligible.Count == 0) return null;

        float t = Random.value * total;
        for (int i = 0; i < _cacheEligible.Count; i++)
        {
            TunnelSectionSO d = _cacheEligible[i];
            t -= d.spawnWeight;
            if (t <= 0f) return d;
        }
        return _cacheEligible[_cacheEligible.Count - 1];
    }

    // ---------- Speed Drift ----------

    private void UpdateSpeedDrift()
    {
        if (_drifting)
        {
            _driftT += Time.deltaTime / Mathf.Max(0.001f, _driftDuration);

            float s = _driftT;
            s = s * s * (3f - 2f * s); // smoothstep

            float newSpeed = Mathf.Lerp(_currentSpeed, _targetSpeed, s);
            _currentSpeed = Mathf.Clamp(newSpeed, speedRange.x, speedRange.y);

            if (_driftT >= 1f)
            {
                _drifting = false;
                _driftIdleTimer = Random.Range(driftIdleIntervalRange.x, driftIdleIntervalRange.y);
            }
        }
        else
        {
            _driftIdleTimer -= Time.deltaTime;
            if (_driftIdleTimer <= 0f)
            {
                _drifting = true;
                _driftT = 0f;
                _driftDuration = Random.Range(driftRampDurationRange.x, driftRampDurationRange.y);

                float newTarget;
                int safety = 0;
                do
                {
                    newTarget = Random.Range(speedRange.x, speedRange.y);
                    safety++;
                } while (Mathf.Abs(newTarget - _currentSpeed) < 0.15f && safety < 8);

                _targetSpeed = newTarget;
            }
        }
    }

    // ---------- Gizmos ----------

    private void OnDrawGizmosSelected()
    {
        if (tunnelParent == null) return;

        // Despawn line
        Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.6f);
        Gizmos.DrawLine(new Vector3(-5f, despawnBelow, -5f), new Vector3(5f, despawnBelow, 5f));
        Gizmos.DrawLine(new Vector3(-5f, despawnBelow, 5f), new Vector3(5f, despawnBelow, -5f));

        // Spawn window indicator at current top
        float topY = GetNextBaseWorldY();
        Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.6f);
        Gizmos.DrawLine(new Vector3(-5f, topY, -5f), new Vector3(5f, topY, 5f));
        Gizmos.DrawLine(new Vector3(-5f, topY, 5f), new Vector3(5f, topY, -5f));

        // Target window ceiling
        Gizmos.color = new Color(0.2f, 1f, 0.2f, 0.4f);
        Gizmos.DrawLine(new Vector3(-7f, spawnWindowAhead, -7f), new Vector3(7f, spawnWindowAhead, 7f));
        Gizmos.DrawLine(new Vector3(-7f, spawnWindowAhead, 7f), new Vector3(7f, spawnWindowAhead, -7f));
    }
}