﻿using System.Collections;
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

public class DialogueNode : Node
{
    private const string DIALOGUE = "Dialogue";
    private const string DIALOGUE_CHOICE = "Dialogue Choice";
    private const string BATTLENODE = "Battle Node";
    private const string ENDNODE = "End Node";
    private const string EVENTNODE = "Event";


    public Action<DialogueNode> OnNodeSelected;

    public string GUID;
    public string dialogueText;
    public string nodeType;
    public Modifier modifier;
    public List<DialogueChoices> choices = new List<DialogueChoices>();
    public string eventName;

    DialogueGraphView graphView;
    Editor editor;

    #region Constructors

    public DialogueNode(string GUID, DialogueGraphView graphView, string nodeType, Modifier modifier, Action<DialogueNode> nodeSelected)
    {
        this.GUID = GUID;
        this.graphView = graphView;
        this.nodeType = nodeType;
        this.modifier = modifier;
        OnNodeSelected = nodeSelected;


        mainContainer.AddToClassList("ds-node__main-container");
        extensionContainer.AddToClassList("ds-node__extension-container");
    }

    public DialogueNode(string GUID, DialogueGraphView graphView, string nodeType, bool isChoice, Modifier modifier, Action<DialogueNode> nodeSelected)
    {
        this.GUID = GUID;
        this.graphView = graphView;
        this.nodeType = nodeType;
        this.modifier = modifier;
        OnNodeSelected = nodeSelected;

        mainContainer.AddToClassList("ds-node__main-container");
        extensionContainer.AddToClassList("ds-node__extension-container");

        choices.Add(new DialogueChoices("New Choice", Guid.NewGuid().ToString()));
    }

    public DialogueNode(string GUID, DialogueGraphView graphView, string nodeType, Action<DialogueNode> nodeSelected, string eventName = "Event")
    {
        this.GUID = GUID;
        this.graphView = graphView;
        this.nodeType = nodeType;
        this.eventName = eventName;
        OnNodeSelected = nodeSelected;

        mainContainer.AddToClassList("ds-node__main-container");
        extensionContainer.AddToClassList("ds-node__extension-container");
    }

    #endregion

    #region Draw Node

    public void Draw(Vector2 position, Vector2 size)
    {
        CreateLabel();

        CreateDialogueTextField();

        CreateInspector();

        CreatePorts();

        if (choices.Count > 0)
            CreateChoices();

        // Refresh node
        RefreshExpandedState();
        RefreshPorts();

        SetPosition(new Rect(position, size));
    }

    public void DrawUtility(Vector2 position, Vector2 size)
    {
        CreateLabel();

        CreatePorts();

        CreateInspector();


        // Refresh node
        RefreshExpandedState();
        RefreshPorts();

        SetPosition(new Rect(position, size));
    }

    public void DrawEvent(Vector2 position, Vector2 size)
    {
        CreatePorts();

        CreateEventOptions();

        // Refresh node
        RefreshExpandedState();
        RefreshPorts();

        SetPosition(new Rect(position, size));
    }

    #endregion

    private void CreateLabel()
    {
        Label label = new Label();
        label.text = nodeType;


        label.AddToClassList("ds-node_Label");

        titleContainer.Insert(0, label);
    }
    
    private void CreatePorts()
    {
        // Add ports
        Port inputPort = nodeType == EVENTNODE ? InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(float)) : 
                                                 InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(float));
        inputPort.portName = "Input";
        inputPort.name = "input";
        inputContainer.Add(inputPort);

        if (choices.Count > 0 || nodeType == "End Node")
            return;

        Port outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(float));
        outputPort.portName = "Output";
        outputPort.name = "output";
        outputContainer.Add(outputPort);
    }

    private void CreateDialogueTextField()
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

    private void CreateInspector()
    {
        if (modifier == null)
            return;

        VisualElement customDataContainer = new VisualElement();
        customDataContainer.AddToClassList("ds-node__custom-data-container");

        Foldout textFolout = new Foldout();
        textFolout.text = "Inspector";

        textFolout.Clear();
        UnityEngine.Object.DestroyImmediate(editor);
        editor = Editor.CreateEditor(modifier);
        IMGUIContainer container = new IMGUIContainer(() => { editor.OnInspectorGUI(); });


        textFolout.Add(container);
        customDataContainer.Add(textFolout);
        extensionContainer.Add(customDataContainer);
    }

    private void CreateChoices()
    {
        Button addButton = CreateButton("Add Choice", () =>
        {
            string newGUID = Guid.NewGuid().ToString();

            DialogueChoices choice = new DialogueChoices("New Choice", newGUID);

            Port port = CreateChoicePort(choice, newGUID);

            choices.Add(choice);

            outputContainer.Add(port);
        });

        addButton.AddToClassList("ds-node__button");

        mainContainer.Insert(1, addButton);

        foreach (DialogueChoices choice in choices)
        {
            Port port = CreateChoicePort(choice, choice.portGUID);

            outputContainer.Add(port);
        }
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
            graphView.LoadEvent(GUID, eventName);
        });

        button.AddToClassList("ds-node__button");

        titleContainer.Insert(0, textArea);
        extensionContainer.Add(button);
    }


    Button CreateButton(string text, Action onClick = null)
    {
        Button button = new Button(onClick);
        button.text = text;

        return button;
    }

    Port CreateChoicePort(DialogueChoices choice, string guid)
    {
        Port port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(float));
        port.portName = "";
        port.name = guid;

        Button deleteButton = CreateButton("X", () =>
        {
            if (choices.Count == 1)
                return;

            if (port.connected)
                graphView.DeleteElements(port.connections);

            choices.Remove(choice);


            graphView.RemoveElement(port);
        });

        deleteButton.AddToClassList("ds-node__button");

        TextField choiceTextField = CreateTextField(choice.text, callback =>
        {
            choice.text = callback.newValue;
        });

        choiceTextField.AddToClassList("ds-node__textfield");
        choiceTextField.AddToClassList("ds-node__choice-textfield");
        choiceTextField.AddToClassList("ds-node__textfield__hidden");

        port.Add(choiceTextField);
        port.Add(deleteButton);
        return port;
    }

    TextField CreateTextField(string text, EventCallback<ChangeEvent<string>> onValueChanged = null)
    {
        TextField textField = new TextField();
        textField.value = text;

        if (onValueChanged != null)
        {
            textField.RegisterValueChangedCallback(onValueChanged);
        }

        return textField;
    }

    TextField CreateTextArea(string text, EventCallback<ChangeEvent<string>> onValueChanged = null)
    {
        TextField textArea = CreateTextField(text, onValueChanged);

        textArea.multiline = true;

        return textArea;
    }

    public override void OnSelected()
    {
        if (nodeType == ENDNODE || nodeType == EVENTNODE)
            return;

        base.OnSelected();
        if (OnNodeSelected != null)
        {
            OnNodeSelected.Invoke(this);
        }
    }
}
