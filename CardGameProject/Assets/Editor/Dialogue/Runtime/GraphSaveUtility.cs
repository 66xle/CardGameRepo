using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using System;
using System.Web;

public class GraphSaveUtility
{
    private const string DIALOGUE = "Dialogue";
    private const string DIALOGUE_CHOICE = "Dialogue Choice";
    private const string BATTLENODE = "Battle Node";
    private const string LINKEDNODE = "Linked Node";

    private DialogueGraphView _targetGraphView;
    private Event _containerCache;

    private List<Edge> Edges => _targetGraphView.edges.ToList();
    private List<EventNode> Nodes => _targetGraphView.nodes.ToList().Cast<EventNode>().ToList();

    private DialogueNodeData startNode;

    List<Port> Ports => _targetGraphView.ports.ToList();

    public static GraphSaveUtility GetInstance(DialogueGraphView targetGraphView)
    {
        return new GraphSaveUtility
        {
            _targetGraphView = targetGraphView
        };
    }
    
    #region SaveGraph

    public void SaveGraph(string fileName)
    {
        // If there are no connections don't save
        if (!Edges.Any() && _targetGraphView.isInEventState)
        {
            EditorUtility.DisplayDialog("Error", "No connections detected!", "OK");
            return;
        }

        List<Port> inputPorts = Ports.Where(x => x.name == "input").ToList();
        List<Port> portsNotConnected = inputPorts.Where(x => x.connected == false).ToList();

        // Checks if there are one input node not connected
        if (portsNotConnected.Count > 1)
        {
            EditorUtility.DisplayDialog("Error", "Only one node should have no input connection", "OK");
            return;
        }


        Event eventContainer = GetEventData();
        

        if (eventContainer != null)
            SaveEventAsset(fileName, eventContainer);
    }

    public Event GetEventData()
    {
        if (Nodes.Count == 0)
            return null;

        #region Check for starting node

        List<Port> inputPorts = Ports.Where(x => x.name == "input").ToList();
        List<Port> portsNotConnected = inputPorts.Where(x => x.connected == false).ToList();
        EventNode node = portsNotConnected[0].node as EventNode;

        startNode = new DialogueNodeData
        {
            Guid = node._GUID,
        };

        #endregion

        Event eventContainer = ScriptableObject.CreateInstance<Event>();

        #region Get Connections

        List<NodeLinkData> nodeLinks = new List<NodeLinkData>();

        // Get edges that are connected to an input port
        Edge[] connectedPorts = Edges.Where(x => x.input.node != null).ToArray();
        for (int i = 0; i < connectedPorts.Count(); i++)
        {
            // Get both nodes the edge is connected to
            EventNode outputNode = (connectedPorts[i].output.node as EventNode);
            EventNode inputNode = (connectedPorts[i].input.node as EventNode);

            // Create connection data
            nodeLinks.Add(new NodeLinkData
            {
                BaseNodeGuid = outputNode._GUID,
                PortName = connectedPorts[i].output.portName,
                TargetNodeGuid = inputNode._GUID,
                ChoiceGUID = connectedPorts[i].output.name,
            }); ;
        }


        #endregion

        #region Save Nodes

        // Save all nodes
        foreach (EventNode eventNode in Nodes)
        {
            // Cloning choices so there won't be a reference in scriptable object
            List<DialogueChoices> choices = new List<DialogueChoices>();

            if (eventNode._nodeType == DIALOGUE_CHOICE)
            {
                foreach (DialogueChoices choice in ((DialogueChoiceNode)eventNode).choices)
                {
                    DialogueChoices choiceData = new DialogueChoices(choice.text, choice.portGUID);
                    choiceData.targetGUID = choice.targetGUID;

                    choices.Add(choiceData);
                }
            }
            

            DialogueNodeData newNode = new DialogueNodeData
            {
                Guid = eventNode._GUID,
                Position = eventNode.GetPosition().position,
                Connections = nodeLinks.Where(x => x.BaseNodeGuid == eventNode._GUID).ToList(),
                Choices = choices,
                NodeType = eventNode._nodeType,
            };

            // Store variables
            if (eventNode._nodeType == DIALOGUE)
            {
                newNode.DialogueText = ((DialogueNode)eventNode).dialogueText;
            }
            else if (eventNode._nodeType == DIALOGUE_CHOICE)
            {
                newNode.DialogueText = ((DialogueChoiceNode)eventNode).dialogueText;
            }
            else if (eventNode._nodeType == LINKEDNODE)
            {
                newNode.eventName = ((LinkedNode)eventNode).eventName;
            }


            // Store modifier
            if (eventNode._nodeType == BATTLENODE)
            {
                BattleModifier batMod = eventNode._modifier as BattleModifier;
                newNode.enemies = batMod.enemies;
                newNode.cards = batMod.cards;
                newNode.money = batMod.money;
            }
            else if (eventNode._nodeType == DIALOGUE || eventNode._nodeType == DIALOGUE_CHOICE)
            {
                DialogueModifier diaMod = eventNode._modifier as DialogueModifier;
                newNode.cards = diaMod.cards;
                newNode.money = diaMod.money;
                newNode.image = diaMod.image;
            }

            // Starting node
            if (eventNode._GUID == startNode.Guid)
            {
                newNode.isStartNode = true;
            }


            eventContainer.DialogueNodeData.Add(newNode);
        }

        #endregion

        return eventContainer;
    }

    

    private void SaveEventAsset(string fileName, Event eventContainer)
    {
        // Check if asset exists

        Event loadedAsset = AssetDatabase.LoadAssetAtPath($"Assets/ScriptableObjects/Events/{fileName}.asset", typeof(Event)) as Event;

        if (loadedAsset != null)
        {
            // If false cancel
            if (!EditorUtility.DisplayDialog($"Save {fileName}", $"Save \"{fileName}\" event?", "Save", "Cancel"))
                return;

            // is linked or event state
            if (_targetGraphView.isInEventState)
            {
                SetDataInObject(loadedAsset, eventContainer);
            }
            else
            {
                loadedAsset.DialogueNodeData.Clear();
                loadedAsset.DialogueNodeData = new List<DialogueNodeData>(eventContainer.DialogueNodeData);
            }

            // Check if event is linked and if child data is greater than node count
            if (loadedAsset.type == "Linked Event")
            {
                bool isThereDataToRemove = true;
                ChildEventData dataToRemove = null;


                // Remove data that were deleted from graph
                for (int i = 0; i < loadedAsset.listChildData.Count; i++)
                {
                    isThereDataToRemove = true;
                    dataToRemove = loadedAsset.listChildData[i];

                    foreach (DialogueNodeData data in loadedAsset.DialogueNodeData)
                    {
                        if (data.Guid == loadedAsset.listChildData[i].guid)
                        {
                            isThereDataToRemove = false;
                            dataToRemove = null;
                            break;
                        }
                    }

                    if (isThereDataToRemove)
                    {
                        loadedAsset.listChildData.Remove(dataToRemove);
                    }
                }
            }
            

            EditorUtility.SetDirty(loadedAsset);
        }
        else
        {
            // Never will run
            //AssetDatabase.CreateAsset(eventContainer, $"Assets/ScriptableObjects/Events/{fileName}.asset");
        }

        AssetDatabase.SaveAssets();
    }

    private void SetDataInObject(Event loadedAsset, Event eventContainer)
    {
        if (loadedAsset.type == "Single Event")
        {
            loadedAsset.DialogueNodeData.Clear();
            loadedAsset.DialogueNodeData = new List<DialogueNodeData>(eventContainer.DialogueNodeData);
        }
        else if (loadedAsset.type == "Linked Event")
        {
            // Loop to find child data
            for (int i = 0; i < loadedAsset.listChildData.Count; i++)
            {
                // If guid matches replace with new dialouge node data
                if (loadedAsset.listChildData[i].guid == _targetGraphView.openedEventGUID)
                {
                    loadedAsset.listChildData[i].dialogueNodeData = eventContainer.DialogueNodeData;
                    return;
                }
            }

            // Create new data
            ChildEventData newData = new ChildEventData();
            newData.guid = _targetGraphView.openedEventGUID;
            newData.dialogueNodeData = eventContainer.DialogueNodeData;

            loadedAsset.listChildData.Add(newData);

            loadedAsset.DialogueNodeData.Clear();
            loadedAsset.DialogueNodeData = new List<DialogueNodeData>(_targetGraphView.linkedEvent.DialogueNodeData);
        }
    }

    public Event GetDataFromObject(string fileName, string nodeGUID)
    {
        Event tempEvent = new Event();

        Event loadedAsset = AssetDatabase.LoadAssetAtPath($"Assets/ScriptableObjects/Events/{fileName}.asset", typeof(Event)) as Event;

        // Loop to find child data
        for (int i = 0; i < loadedAsset.listChildData.Count; i++)
        {
            // If guid matches
            if (loadedAsset.listChildData[i].guid == nodeGUID)
            {
                tempEvent.DialogueNodeData = loadedAsset.listChildData[i].dialogueNodeData;
                tempEvent.name = fileName;
                break;
            }
        }

        return tempEvent;
    }

    #endregion

    #region LoadGraph

    public void LoadGraph(Event eventToLoad)
    {
        _containerCache = eventToLoad;

        ClearGraph();
        CreateNodes();

        ConnectNodes();
    }

    public void ClearGraph()
    {
        // Set entry points guid back from the save. Discard existing guid
        //Nodes.Find(x => x.EntryPoint).GUID = _containerCache.NodeLinks[0].BaseNodeGuid;

        foreach (EventNode node in Nodes)
        {
            // Remove edges that is connected to this node
            Edges.Where(x => x.input.node == node).ToList().ForEach(edge => _targetGraphView.RemoveElement(edge));

            // Then remove node
            _targetGraphView.RemoveElement(node);
        }

    }

    private void CreateNodes()
    {
        foreach (DialogueNodeData nodeData in _containerCache.DialogueNodeData)
        {
            if (nodeData.NodeType == DIALOGUE)
            {
                DialogueNode dialogueNode = new DialogueNode(nodeData.Guid, nodeData.NodeType, _targetGraphView, new DialogueModifier(nodeData.cards, nodeData.money, nodeData.image), _targetGraphView.OnNodeSelected);
                dialogueNode.dialogueText = nodeData.DialogueText;
                dialogueNode.Draw(nodeData.Position, _targetGraphView.DefaultNodeSize);
                _targetGraphView.AddElement(dialogueNode);
            }
            else if (nodeData.NodeType == DIALOGUE_CHOICE)
            {
                DialogueChoiceNode choiceNode = new DialogueChoiceNode(nodeData.Guid, nodeData.NodeType, _targetGraphView, new DialogueModifier(nodeData.cards, nodeData.money, nodeData.image), _targetGraphView.OnNodeSelected);
                choiceNode.choices = nodeData.Choices;
                choiceNode.dialogueText = nodeData.DialogueText;
                choiceNode.Draw(nodeData.Position, _targetGraphView.DefaultNodeSize);
                _targetGraphView.AddElement(choiceNode);
            }
            else if (nodeData.NodeType == BATTLENODE)
            {
                BattleNode battleNode = new BattleNode(nodeData.Guid, nodeData.NodeType, _targetGraphView, new BattleModifier(nodeData.enemies, nodeData.cards, nodeData.money), _targetGraphView.OnNodeSelected);
                battleNode.Draw(nodeData.Position, _targetGraphView.DefaultNodeSize);
                _targetGraphView.AddElement(battleNode);
            }
            else if (nodeData.NodeType == LINKEDNODE)
            {
                LinkedNode linkedNode = new LinkedNode(nodeData.Guid, nodeData.NodeType, _targetGraphView, _targetGraphView.OnNodeSelected);
                linkedNode.Draw(nodeData.Position, _targetGraphView.DefaultNodeSize);
                _targetGraphView.AddElement(linkedNode);
            }
        }
    }

    private void ConnectNodes()
    {
        // Loop through nodes on graph
        for (int i = 0; i < Nodes.Count; i++)
        {
            // Get all output connections connected to this node
            List<NodeLinkData> connections = _containerCache.DialogueNodeData.First(x => x.Guid == Nodes[i]._GUID).Connections;
            for (int j = 0; j < connections.Count; j++)
            {
                string targetNodeGuid = connections[j].TargetNodeGuid;
                EventNode targetNode = Nodes.First(x => x._GUID == targetNodeGuid);

                // Link ports
                LinkNodes(Nodes[i].outputContainer[j].Q<Port>(), (Port)targetNode.inputContainer[0]);
            }
        }
    }

    private void LinkNodes(Port output, Port input)
    {
        Edge tempEdge = new Edge
        {
            output = output,
            input = input
        };

        tempEdge?.input.Connect(tempEdge);
        tempEdge?.output.Connect(tempEdge);

        _targetGraphView.Add(tempEdge);
    }

    #endregion
}
