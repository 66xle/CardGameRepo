using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EndlessTunnelManagerUnified : MonoBehaviour
{
    public enum TunnelMode { Organic, Arriving, Cutscene, Paused, Resuming }

    [Header("Setup")]
    public Transform TunnelParent;
    public List<TunnelSectionSO> SectionLibrary = new List<TunnelSectionSO>();
    public List<TunnelSectionSO> ArenaLibrary = new List<TunnelSectionSO>();
    public int InitialSections = 6;

    [Header("Spawn/Despawn")]
    public float SpawnWindowAhead = 80f;
    public float DespawnBelow = -80f;

    [Header("Organic Speed Drift")]
    public Vector2 SpeedRange = new Vector2(1.5f, 3f);
    public Vector2 DriftIdleIntervalRange = new Vector2(6f, 14f);
    public Vector2 DriftRampDurationRange = new Vector2(5f, 10f);

    [Header("Arena Arrival")]
    public float ArenaDecelDuration = 4f;
    public UnityEvent OnArenaArrived;

    [Header("Cutscene Events")]
    public UnityEvent OnCutsceneStarted;

    [Header("Resume Events")]
    public UnityEvent OnResumed;

    [Header("Resume Settings")]
    public float ResumeDuration = 3f;

    // --- Internal ---
    private TunnelMode _mode = TunnelMode.Organic;
    private float _currentSpeed;
    private float _targetSpeed;

    // Organic drift
    private bool _drifting;
    private float _driftT;
    private float _driftDuration;
    private float _driftIdleTimer;

    // Arena arrival
    private bool _arriving;
    private TunnelSectionSO _arrivalDef;
    private float _arrivalStartSpeed;
    private float _arrivalTimer;
    private float _anchorPlaneY = 0f;

    // Cutscene state
    private TunnelSectionSO _cutsceneTarget;
    private float _cutsceneStopTime;
    private float _cutsceneDecelDuration;
    private float _cutsceneElapsed;
    private float _cutsceneBaseSpeed;
    private bool _organicSpawningAfterCutscene;

    // Resume state
    private float _resumeTimer;
    private float _resumeTargetSpeed;

    // Active spawned sections
    private struct Spawned
    {
        public GameObject Go;
        public float LocalY;
        public float Height;
        public TunnelSectionSO Def;
    }
    private readonly List<Spawned> _active = new List<Spawned>();
    private float _nextStackY;

    void Start()
    {
        if (TunnelParent == null) TunnelParent = transform;
        _currentSpeed = (SpeedRange.x + SpeedRange.y) * 0.5f;
        ResetAndPrewarm();
    }

    void Update()
    {
        switch (_mode)
        {
            case TunnelMode.Cutscene: UpdateCutsceneMode(); break;
            case TunnelMode.Arriving: UpdateArrival(); break;
            case TunnelMode.Resuming: UpdateResuming(); break;
            case TunnelMode.Organic:  UpdateOrganicDrift(); break;
            case TunnelMode.Paused:   _currentSpeed = 0f; break;
        }

        TunnelParent.position += Vector3.down * (_currentSpeed * Time.deltaTime);

        // Spawning rules
        if (_mode == TunnelMode.Cutscene)
        {
            if (_organicSpawningAfterCutscene)
                MaintainSpawnWindow(); // keep spawning organically after arena
        }
        else
        {
            MaintainSpawnWindow();
            DespawnPassed();
        }
    }

    // ---------------- Public API ----------------
    public void ArriveAtArena(int arenaIndex)
    {
        if (arenaIndex < 0 || arenaIndex >= ArenaLibrary.Count) return;

        _arrivalDef = ArenaLibrary[arenaIndex];
        SpawnNext(_arrivalDef);

        _arriving = true;
        _arrivalStartSpeed = _currentSpeed;
        _arrivalTimer = 0f;
        _mode = TunnelMode.Arriving;
    }

    public void ResumeTunnel()
    {
        if (_mode != TunnelMode.Paused && _mode != TunnelMode.Cutscene) return;
        _resumeTimer = 0f;
        _resumeTargetSpeed = Random.Range(SpeedRange.x, SpeedRange.y);
        _mode = TunnelMode.Resuming;
    }

    public void PlanCutsceneArrival(List<TunnelSectionSO> sequence, float stopTime, float decelDuration, bool append = false)
    {
        if (sequence == null || sequence.Count == 0) return;

        _mode = TunnelMode.Cutscene;
        _cutsceneTarget = sequence[sequence.Count - 1];
        _cutsceneStopTime = Mathf.Max(0.01f, stopTime);
        _cutsceneDecelDuration = Mathf.Clamp(decelDuration, 0.01f, _cutsceneStopTime - 0.001f);
        _cutsceneElapsed = 0f;
        _organicSpawningAfterCutscene = false;

        if (!append)
            ResetForCutscene();

        // Spawn all cutscene sections
        List<Spawned> spawnedSequence = new List<Spawned>();
        foreach (var def in sequence)
            spawnedSequence.Add(SpawnNextCutscene(def));

        // Mark that organic should continue spawning beyond the cutscene stack
        _organicSpawningAfterCutscene = true;

        // Measure distance
        Spawned target = spawnedSequence[spawnedSequence.Count - 1];
        Vector3 stopPoint = TunnelParent.TransformPoint(new Vector3(0f, target.LocalY + target.Def.stopOffset, 0f));
        float distance = stopPoint.y - _anchorPlaneY;

        // Base speed formula (integral of decel = 0.25)
        float cruiseTime = _cutsceneStopTime - _cutsceneDecelDuration;
        float effectiveTime = Mathf.Max(0.01f, cruiseTime + _cutsceneDecelDuration * 0.25f);

        _cutsceneBaseSpeed = distance / effectiveTime;
        _currentSpeed = _cutsceneBaseSpeed;

        OnCutsceneStarted?.Invoke();
    }

    // ---------------- Updates ----------------
    private void UpdateOrganicDrift()
    {
        if (_drifting)
        {
            _driftT += Time.deltaTime / Mathf.Max(0.001f, _driftDuration);
            float s = _driftT * _driftT * (3f - 2f * _driftT);
            _currentSpeed = Mathf.Lerp(_currentSpeed, _targetSpeed, s);

            if (_driftT >= 1f)
            {
                _drifting = false;
                _driftIdleTimer = Random.Range(DriftIdleIntervalRange.x, DriftIdleIntervalRange.y);
            }
        }
        else
        {
            _driftIdleTimer -= Time.deltaTime;
            if (_driftIdleTimer <= 0f)
            {
                _drifting = true;
                _driftT = 0f;
                _driftDuration = Random.Range(DriftRampDurationRange.x, DriftRampDurationRange.y);

                float newTarget;
                int safety = 0;
                do { newTarget = Random.Range(SpeedRange.x, SpeedRange.y); safety++; }
                while (Mathf.Abs(newTarget - _currentSpeed) < 0.15f && safety < 8);

                _targetSpeed = newTarget;
            }
        }
    }

    private void UpdateArrival()
    {
        foreach (var s in _active)
        {
            if (s.Def == _arrivalDef)
            {
                float stopWorldY = TunnelParent.TransformPoint(new Vector3(0, s.LocalY + _arrivalDef.stopOffset, 0)).y;
                if (stopWorldY > _anchorPlaneY) return;

                _arrivalTimer += Time.deltaTime;
                float t = Mathf.Clamp01(_arrivalTimer / ArenaDecelDuration);
                _currentSpeed = Mathf.Lerp(_arrivalStartSpeed, 0f, t * t * (3f - 2f * t));

                if (_currentSpeed <= 0.01f)
                {
                    _currentSpeed = 0f;
                    _arriving = false;
                    _mode = TunnelMode.Paused;
                    OnArenaArrived?.Invoke();
                }
                break;
            }
        }
    }

    private void UpdateCutsceneMode()
    {
        _cutsceneElapsed += Time.deltaTime;

        if (_cutsceneElapsed < _cutsceneStopTime - _cutsceneDecelDuration)
        {
            _currentSpeed = _cutsceneBaseSpeed;
        }
        else
        {
            float t = Mathf.Clamp01((_cutsceneElapsed - (_cutsceneStopTime - _cutsceneDecelDuration)) / _cutsceneDecelDuration);
            float ease = 1f - Mathf.Pow(1f - t, 3f);
            _currentSpeed = Mathf.Lerp(_cutsceneBaseSpeed, 0f, ease);

            if (_cutsceneElapsed >= _cutsceneStopTime)
            {
                _currentSpeed = 0f;
                _mode = TunnelMode.Paused;
                OnArenaArrived?.Invoke();
            }
        }
    }

    private void UpdateResuming()
    {
        _resumeTimer += Time.deltaTime;
        float t = Mathf.Clamp01(_resumeTimer / ResumeDuration);
        float ease = t * t * (3f - 2f * t);
        _currentSpeed = Mathf.Lerp(0f, _resumeTargetSpeed, ease);

        if (t >= 1f)
        {
            _mode = TunnelMode.Organic;
            _drifting = false;
            OnResumed?.Invoke();
        }
    }

    // ---------------- Spawning Helpers ----------------
    private void ResetAndPrewarm()
    {
        for (int i = _active.Count - 1; i >= 0; i--)
            if (_active[i].Go) Destroy(_active[i].Go);
        _active.Clear();
        _nextStackY = 0f;

        for (int i = 0; i < InitialSections; i++)
            SpawnNext(null);
    }

    private void ResetForCutscene()
    {
        for (int i = _active.Count - 1; i >= 0; i--)
            if (_active[i].Go) Destroy(_active[i].Go);
        _active.Clear();
        _nextStackY = 0f;
    }

    private void MaintainSpawnWindow()
    {
        float topWorldY = GetNextBaseWorldY();
        int safety = 0;
        while (topWorldY < SpawnWindowAhead && safety++ < 100)
        {
            SpawnNext(null);
            topWorldY = GetNextBaseWorldY();
        }
    }

    private void DespawnPassed()
    {
        for (int i = _active.Count - 1; i >= 0; i--)
        {
            Spawned s = _active[i];
            float topLocalY = s.LocalY + s.Height;
            float topWorldY = TunnelParent.TransformPoint(new Vector3(0f, topLocalY, 0f)).y;
            if (topWorldY < DespawnBelow)
            {
                if (s.Go) Destroy(s.Go);
                _active.RemoveAt(i);
            }
        }
    }

    private Spawned SpawnNext(TunnelSectionSO forced)
    {
        TunnelSectionSO def = forced ?? PickDefinition();
        if (def == null || def.prefab == null) return default;

        GameObject go = Instantiate(def.prefab, TunnelParent);
        go.transform.localPosition = new Vector3(0f, _nextStackY, 0f);

        float step = Mathf.Max(1f, def.rotationStepDegrees);
        int steps = Mathf.Max(1, Mathf.RoundToInt(360f / step));
        int r = Random.Range(0, steps);
        go.transform.localRotation = Quaternion.Euler(0f, r * step, 0f);

        float h = Mathf.Max(0.01f, def.EffectiveHeight);
        Spawned spawned = new Spawned { Go = go, LocalY = _nextStackY, Height = h, Def = def };
        _active.Add(spawned);
        _nextStackY += h;
        return spawned;
    }

    private Spawned SpawnNextCutscene(TunnelSectionSO def)
    {
        if (def == null || def.prefab == null) return default;

        GameObject go = Instantiate(def.prefab, TunnelParent);
        go.transform.localPosition = new Vector3(0f, _nextStackY, 0f);
        go.transform.localRotation = Quaternion.Euler(0f, def.initialRotation, 0f);

        float h = Mathf.Max(0.01f, def.EffectiveHeight);
        Spawned spawned = new Spawned { Go = go, LocalY = _nextStackY, Height = h, Def = def };
        _active.Add(spawned);
        _nextStackY += h;
        return spawned;
    }

    private TunnelSectionSO PickDefinition()
    {
        float total = 0f;
        List<TunnelSectionSO> eligible = new List<TunnelSectionSO>();
        foreach (var d in SectionLibrary)
        {
            if (d == null || d.prefab == null) continue;
            if (d.minProgressLevel > 0) continue;
            if (d.spawnWeight <= 0f) continue;
            eligible.Add(d);
            total += d.spawnWeight;
        }
        if (eligible.Count == 0) return null;

        float t = Random.value * total;
        foreach (var d in eligible)
        {
            t -= d.spawnWeight;
            if (t <= 0f) return d;
        }
        return eligible[eligible.Count - 1];
    }

    private float GetNextBaseWorldY()
    {
        return TunnelParent.TransformPoint(new Vector3(0f, _nextStackY, 0f)).y;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 left = new Vector3(-10f, _anchorPlaneY, 0f);
        Vector3 right = new Vector3(10f, _anchorPlaneY, 0f);
        Gizmos.DrawLine(left, right);
    }
}