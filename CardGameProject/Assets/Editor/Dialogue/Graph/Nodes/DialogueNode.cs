using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.Port;
using UnityEditor.UIElements;
using System;
using static DialogueNode;
using System.Reflection.Emit;
using Label = UnityEngine.UIElements.Label;
using UnityEngine.Experimental.AI;
using UnityEditorInternal.VR;

public class DialogueNode : EventNode
{
    public string dialogueText;

    public DialogueNode(string GUID, string nodeType, DialogueGraphView graphView , Modifier modifier, Action<EventNode> nodeSelected)
    {
        _GUID = GUID;
        _nodeType = nodeType;
        _graphView = graphView;
        _modifier = modifier;
        OnNodeSelected = nodeSelected;
    }

    #region Draw Node

    public void Draw(Vector2 position, Vector2 size)
    {
        CreateLabelTitle();

        CreatePorts();

        CreateDialogueTextField();

        CreateInspector();


        // Refresh node
        RefreshExpandedState();
        RefreshPorts();
        SetPosition(new Rect(position, size));
    }


    #endregion


    protected void CreateDialogueTextField()
    {
        VisualElement customDataContainer = new VisualElement();

        customDataContainer.AddToClassList("ds-node__custom-data-container");

        Foldout textFolout = new Foldout();
        textFolout.text = "Dialogue Text";

        TextField textArea = CreateTextArea(dialogueText, callback =>
        {
            dialogueText = callback.newValue;
        });

        textArea.AddToClassList("ds-node__textfield");
        textArea.AddToClassList("ds-node__quote-textfield");


        textFolout.Add(textArea);
        customDataContainer.Add(textFolout);
        extensionContainer.Add(customDataContainer);
    }






















}
