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

        CreateDialogueTextField(dialogueText);

        CreateInspector();


        // Refresh node
        RefreshExpandedState();
        RefreshPorts();
        SetPosition(new Rect(position, size));
    }


    #endregion

    
    
    

    

    

    

    


    

    

    

    

    
}
