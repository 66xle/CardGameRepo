using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEditor;
using UnityEngine;

public enum Effect
{
    Stunned,
    GuardBroken,
    Bleed
}


public class StatusEffect : ScriptableObject
{
    [HideInInspector] public Effect effect;
    public string effectName;
    public int turnsRemaning = 0;
    public GameObject uiPrefab;
}
