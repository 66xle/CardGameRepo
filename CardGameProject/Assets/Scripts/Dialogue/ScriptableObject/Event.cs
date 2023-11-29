using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Event : ScriptableObject
{
    [Header("Dialogue")]
    public List<DialogueNodeData> DialogueNodeData = new List<DialogueNodeData>();
}

