using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Event : ScriptableObject
{
    [Header("Dialogue")]
    public List<DialogueNodeData> DialogueNodeData = new List<DialogueNodeData>();

    [Header("Event Settings")]
    public string name;
    public string description;
    public Sprite image;

    [Header("Enemy Settings")]
    public List<EnemyObj> enemyList;
}

