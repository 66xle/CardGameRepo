using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueNodeData
{
    public string Guid;
    public string DialogueText;
    public string NodeType;
    public Vector2 Position;
    public bool isStartNode;
    public StoreModifier Modifier = new StoreModifier();
    public List<NodeLinkData> Connections;
    public List<DialogueChoices> Choices = new List<DialogueChoices>();

}
