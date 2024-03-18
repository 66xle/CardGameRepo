using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using static UnityEditor.Experimental.GraphView.GraphView;
using System.Runtime.CompilerServices;
using static TreeEditor.TreeEditorHelper;
using System.IO;

public class DialogueGraphView : GraphView
{
    public bool allowCreatingNode = false;
    public bool isInEventState;
    public string openedEventGUID;
    public Label graphTitle;
    public Button backButton;
    public Event linkedEvent;

    public bool hasGraphChanges = false;

    public Action<DialogueNode> OnNodeSelected;

    public const string PLAYER = "Player";
    public const string NPC = "NPC";

    public readonly Vector2 DefaultNodeSize = new Vector2(250f, 200f);

    private NodeSearchWindow _searchWindow;

    public new class UxmlFactory : UxmlFactory<DialogueGraphView, GraphView.UxmlTraits> { }

    public DialogueGraphView()
    {
        // Background lines color
        styleSheets.Add(Resources.Load<StyleSheet>("DialogueGraph"));
        //AddToClassList("node");

        styleSheets.Add(Resources.Load<StyleSheet>("NodeViewStyle"));

        // Allows zoom in and out
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        // Add background lines
        GridBackground grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();

        graphViewChanged = OnGraphChange;
    }

    private GraphViewChange OnGraphChange(GraphViewChange change)
    {
        hasGraphChanges = true;
        Debug.Log("run");

        if (change.edgesToCreate != null)
        {
            // Get edge created
            Edge edge = change.edgesToCreate[0];

            // Get target node
            Node targetNode = edge.input.node;

            // Get base node
            Node baseNode = edge.output.node;

            // Get all target node's input connection
            List<Edge> connections = edges.ToList().Where(x => x.input.node == targetNode).ToList();

            // Check for duplicated edges
            foreach (Edge connection in connections)
            {
                if (connection.output.node == baseNode && connection.input.node == targetNode)
                {
                    EditorUtility.DisplayDialog("Error", "Cannot create duplicate edge!", "OK");
                    change.edgesToCreate.Clear();
                    return change;
                }
            }

            
            DialogueNode bNode = baseNode as DialogueNode;
            if (bNode.choices.Count > 0)
            {
                // Use the connected output port to get choice
                string portGUID = edge.output.name;
                DialogueChoices choice = bNode.choices.First(x => x.portGUID == portGUID);
                
                // Use connected input port to get target guid
                DialogueNode tNode = targetNode as DialogueNode;

                choice.targetGUID = tNode.GUID;
            }
        }

        return change;
    }


    public void LoadEvent(string nodeGUID, string eventName)
    {
        // Pass in linked event guid
        string fileName = Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(openedEventGUID));
        
        // Event node guid linking opened event graph
        openedEventGUID = nodeGUID;

        GraphSaveUtility saveUtility = GraphSaveUtility.GetInstance(this);
        linkedEvent = saveUtility.GetEventData();

        
        Event loadEvent = saveUtility.GetDataFromObject(fileName, nodeGUID);
        saveUtility.LoadGraph(loadEvent);

        graphTitle.text += " >> " + eventName;
        backButton.visible = true;
        isInEventState = true;
    }

    

    #region Search Window

    public void UpdateSearchWindow(DialogueEditor editorWindow)
    {
        if (!allowCreatingNode)
        {
            nodeCreationRequest = null;
            return;
        }

        _searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
        _searchWindow.Configure(editorWindow, this);

        nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
    }

    #endregion

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        List<Port> compatiablePorts = new List<Port>();
        ports.ForEach((port) =>
        {
            if (startPort != port && startPort.node != port.node && startPort.direction != port.direction)
                compatiablePorts.Add(port);
        });

        return compatiablePorts;
    }

    #region Nodes

    public void CreateDialogueNode(Vector2 position, string nodeType, Modifier modifier)
    {
        DialogueNode dialogueNode = new DialogueNode(Guid.NewGuid().ToString(), nodeType, this, modifier, OnNodeSelected);

        dialogueNode.Draw(position, DefaultNodeSize);
        AddElement(dialogueNode);
    }

    public void CreateDialogueChoiceNode(Vector2 position, string nodeType, Modifier modifier)
    {
        DialogueChoiceNode dialogueChoiceNode = new DialogueChoiceNode(Guid.NewGuid().ToString(), nodeType, this, modifier, OnNodeSelected);

        dialogueChoiceNode.Draw(position, DefaultNodeSize);
        AddElement(dialogueChoiceNode);
    }

    public void CreateBattleNode(Vector2 position, string nodeType, Modifier modifier)
    {
        BattleNode battleNode = new BattleNode(Guid.NewGuid().ToString(), nodeType, this, modifier, OnNodeSelected);

        battleNode.Draw(position, DefaultNodeSize);
        AddElement(battleNode);
    }

    public void CreateLinkedNode(Vector2 position, string nodeType)
    {
        LinkedNode linkedNode = new LinkedNode(Guid.NewGuid().ToString(), nodeType, this, OnNodeSelected);

        linkedNode.Draw(position, DefaultNodeSize);
        AddElement(linkedNode);
    }

    #endregion
}
