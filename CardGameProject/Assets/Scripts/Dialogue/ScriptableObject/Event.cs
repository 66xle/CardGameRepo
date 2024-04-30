using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Event : ScriptableObject
{
    [Header("Dialogue")]
    public List<DialogueNodeData> DialogueNodeData = new List<DialogueNodeData>();
    public List<ChildEventData> listChildData = new List<ChildEventData>();
    public int index = 0;
    public string type;
    public string guid; // Sets guid when loading in list
    public string category;
    public Event nextEvent;
}

