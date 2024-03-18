using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleNode : EventNode
{
    public BattleNode(string guid, string nodeType, DialogueGraphView graphView, Modifier modifier, Action<EventNode> nodeSelected)
    {
        _GUID = guid;
        _nodeType = nodeType;
        _graphView = graphView;
        _modifier = modifier;
        OnNodeSelected = nodeSelected;
    }

    public void Draw(Vector2 position, Vector2 size)
    {
        CreateLabelTitle();

        CreatePorts();

        CreateInspector();


        RefreshExpandedState();
        RefreshPorts();
        SetPosition(new Rect(position, size));
    }
}
