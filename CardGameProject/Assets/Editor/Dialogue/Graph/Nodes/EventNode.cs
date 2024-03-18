using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public class EventNode : Node
{
    private const string DIALOGUE = "Dialogue";
    private const string DIALOGUE_CHOICE = "Dialogue Choice";
    private const string BATTLENODE = "Battle Node";
    private const string ENDNODE = "End Node";
    private const string LINKEDNODE = "Linked Node";

    public Action<DialogueNode> OnNodeSelected;

    public string _GUID;
    public string _nodeType;
    public Modifier _modifier;

    protected DialogueGraphView _graphView;
    protected Editor _editor;

    public EventNode()
    {
        mainContainer.AddToClassList("ds-node__main-container");
        extensionContainer.AddToClassList("ds-node__extension-container");
    }

    #region Draw Methods
    protected void CreateLabelTitle()
    {
        Label label = new Label();
        label.text = _nodeType;


        label.AddToClassList("ds-node_Label");

        titleContainer.Insert(0, label);
    }

    protected void CreatePorts()
    {
        // Add ports
        Port inputPort = _nodeType == LINKEDNODE ? InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(float)) :
                                                 InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(float));
        inputPort.portName = "Input";
        inputPort.name = "input";
        inputContainer.Add(inputPort);

        Port outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(float));
        outputPort.portName = "Output";
        outputPort.name = "output";
        outputContainer.Add(outputPort);
    }

    protected void CreateDialogueTextField(string dialogueText)
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

    protected void CreateInspector()
    {
        VisualElement customDataContainer = new VisualElement();
        customDataContainer.AddToClassList("ds-node__custom-data-container");

        Foldout textFolout = new Foldout();
        textFolout.text = "Inspector";

        textFolout.Clear();
        UnityEngine.Object.DestroyImmediate(_editor);
        _editor = Editor.CreateEditor(_modifier);
        IMGUIContainer container = new IMGUIContainer(() => { _editor.OnInspectorGUI(); });


        textFolout.Add(container);
        customDataContainer.Add(textFolout);
        extensionContainer.Add(customDataContainer);
    }

    #endregion

    #region Create Element Method
    protected Button CreateButton(string text, Action onClick = null)
    {
        Button button = new Button(onClick);
        button.text = text;

        return button;
    }

    protected TextField CreateTextField(string text, EventCallback<ChangeEvent<string>> onValueChanged = null)
    {
        TextField textField = new TextField();
        textField.value = text;

        if (onValueChanged != null)
        {
            textField.RegisterValueChangedCallback(onValueChanged);
        }

        return textField;
    }

    protected TextField CreateTextArea(string text, EventCallback<ChangeEvent<string>> onValueChanged = null)
    {
        TextField textArea = CreateTextField(text, onValueChanged);

        textArea.multiline = true;

        return textArea;
    }

    #endregion

    public override void OnSelected()
    {
        if (_nodeType == ENDNODE || _nodeType == LINKEDNODE)
            return;

        base.OnSelected();
        if (OnNodeSelected != null)
        {
            OnNodeSelected.Invoke(this);
        }
    }
}
