﻿using System;
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
    public List<NodeLinkData> Connections;
    public List<DialogueChoices> Choices = new List<DialogueChoices>();


    public List<EnemyData> enemies;
    public List<Card> cards;
    public int money;
    public Sprite image;

    public string eventName;
}
