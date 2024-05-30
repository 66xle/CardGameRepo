using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;


public class DialogueChoiceNode : EventNode
{
    public string dialogueText;
    public List<DialogueChoices> choices = new List<DialogueChoices>();

    public DialogueChoiceNode(string guid, string nodeType, DialogueGraphView graphView, Modifier modifier, Action<EventNode> nodeSelected)
    {
        _GUID = guid;
        _nodeType = nodeType;
        _graphView = graphView;
        _modifier = modifier;
        OnNodeSelected = nodeSelected;

        dialogueText = "Insert Dialouge";
        choices.Add(new DialogueChoices("New Choice", Guid.NewGuid().ToString()));
    }

    public void Draw(Vector2 position, Vector2 size)
    {
        CreateLabelTitle();

        CreateInputPort();

        CreateChoices();

        CreateDialogueTextField();

        CreateInspector();


        RefreshExpandedState();
        RefreshPorts();
        SetPosition(new Rect(position, size));
    }

    private void CreateInputPort()
    {
        Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(float));

        inputPort.portName = "Input";
        inputPort.name = "input";
        inputContainer.Add(inputPort);
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

    #region Create Element Methods

    private Port CreateChoicePort(DialogueChoices choice, string guid)
    {
        Port port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(float));
        port.portName = "";
        port.name = guid;

        Button deleteButton = CreateButton("X", () =>
        {
            if (choices.Count == 1)
                return;

            if (port.connected)
                _graphView.DeleteElements(port.connections);

            choices.Remove(choice);


            _graphView.RemoveElement(port);
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

    #endregion
}
