using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LinkedNode : EventNode
{
    public string eventName;

    public LinkedNode(string guid, string nodeType, DialogueGraphView graphView, Action<DialogueNode> nodeSelected)
    {
        _GUID = guid;
        _nodeType = nodeType;
        _graphView = graphView;
        eventName = "New Event";
        OnNodeSelected = nodeSelected;
    }

    public void Draw(Vector2 position, Vector2 size)
    {
        CreatePorts();

        CreateEventOptions();

        // Refresh node
        RefreshExpandedState();
        RefreshPorts();

        SetPosition(new Rect(position, size));
    }

    private void CreateEventOptions()
    {
        TextField textArea = CreateTextArea(eventName, callback =>
        {
            eventName = callback.newValue;
        });
        eventName = textArea.value;

        textArea.AddToClassList("ds-node__textfield");
        textArea.AddToClassList("ds-node__quote-textfield");

        Button button = CreateButton("Open Event", () =>
        {
            _graphView.LoadEvent(_GUID, eventName);
        });

        button.AddToClassList("ds-node__button");

        titleContainer.Insert(0, textArea);
        extensionContainer.Add(button);
    }
}
