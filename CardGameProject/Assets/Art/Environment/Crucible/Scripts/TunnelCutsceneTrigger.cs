using System.Collections.Generic;
using UnityEngine;

public class TunnelCutsceneHelper : MonoBehaviour
{
    [Header("References")]
    public EndlessTunnelManagerUnified manager;

    [Header("Cutscene Settings")]
    public List<TunnelSectionSO> cutsceneSequence = new List<TunnelSectionSO>();
    public float stopTime = 10f;
    public float decelDuration = 3f;

    [Tooltip("If true, cutscene sequence is appended to existing stack instead of clearing.")]
    public bool appendToExisting = false;

    // Call this from Timeline (via Signal Receiver) or a button
    public void PlayCutscene()
    {
        if (manager != null)
        {
            manager.PlanCutsceneArrival(cutsceneSequence, stopTime, decelDuration, appendToExisting);
        }
    }

    // Optionally resume from Timeline
    public void ResumeTunnel()
    {
        if (manager != null)
        {
            manager.ResumeTunnel();
        }
    }

    // Optionally jump straight to an arena
    public void ArriveAtArena(int arenaIndex)
    {
        if (manager != null)
        {
            manager.ArriveAtArena(arenaIndex);
        }
    }
}