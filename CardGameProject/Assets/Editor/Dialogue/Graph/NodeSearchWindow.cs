using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using static TreeEditor.TreeEditorHelper;

public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider
{
    private EditorWindow _window;
    private DialogueGraphView _graphView;

    private Texture2D _indentationIcon;

    public const string DIALOGUE = "Dialogue";
    public const string DIALOGUE_CHOICE = "Dialogue Choice";
    public const string BATTLENODE = "Battle Node";
    public const string LINKEDNODE = "Linked Node";

    public void Configure(EditorWindow window, DialogueGraphView graphView)
    {
        _window = window;
        _graphView = graphView;

        //Transparent 1px indentation icon as a hack
        _indentationIcon = new Texture2D(1, 1);
        _indentationIcon.SetPixel(0, 0, new Color(0, 0, 0, 0));
        _indentationIcon.Apply();
    }

    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        List<SearchTreeEntry> tree = new List<SearchTreeEntry>
        {
            new SearchTreeGroupEntry(new GUIContent("Nodes"), 0),
            new SearchTreeEntry(new GUIContent(DIALOGUE, _indentationIcon))
            {
                level = 1,
                userData = DIALOGUE
            },
            new SearchTreeEntry(new GUIContent(DIALOGUE_CHOICE, _indentationIcon))
            {
                level = 1,
                userData = DIALOGUE_CHOICE
            },
            new SearchTreeEntry(new GUIContent(BATTLENODE, _indentationIcon))
            {
                level = 1,
                userData = BATTLENODE
            }
        };

        List<SearchTreeEntry> linkedTree = new List<SearchTreeEntry>
        {
            new SearchTreeGroupEntry(new GUIContent("Event Node"), 0),
            new SearchTreeEntry(new GUIContent(LINKEDNODE, _indentationIcon))
            {
                level = 1,
                userData = LINKEDNODE
            },
        };

        if (!_graphView.isInEventState)
            tree = linkedTree;

        return tree;
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        //Editor window-based mouse position
        var mousePosition = _window.rootVisualElement.ChangeCoordinatesTo(_window.rootVisualElement.parent, context.screenMousePosition - _window.position.position);
        var graphMousePosition = _graphView.contentViewContainer.WorldToLocal(mousePosition);
        switch (SearchTreeEntry.userData)
        {
            case DIALOGUE:
                _graphView.CreateDialogueNode(graphMousePosition, DIALOGUE, new DialogueModifier());
                return true;
            case DIALOGUE_CHOICE:
                _graphView.CreateDialogueChoiceNode(graphMousePosition, DIALOGUE_CHOICE, new DialogueModifier());
                return true;
            case BATTLENODE:
                _graphView.CreateBattleNode(graphMousePosition, BATTLENODE, new BattleModifier());
                return true;
            case LINKEDNODE:
                _graphView.CreateLinkedNode(graphMousePosition, LINKEDNODE);
                return true;
        }
        return false;
    }
}
