﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using System;

public class GraphSaveUtility
{
    private DialogueGraphView _targetGraphView;
    private Event _containerCache;

    private List<Edge> Edges => _targetGraphView.edges.ToList();
    private List<DialogueNode> Nodes => _targetGraphView.nodes.ToList().Cast<DialogueNode>().ToList();

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
        if (!Edges.Any())
        {
            EditorUtility.DisplayDialog("Save Failed", "No connections detected!", "OK");
            return;
        }

        #region Check for starting node

        List<Port> inputPorts = Ports.Where(x => x.name == "input").ToList();
        List<Port> portsNotConnected = inputPorts.Where(x => x.connected == false).ToList();

        
        if (portsNotConnected.Count > 1)
        {
            EditorUtility.DisplayDialog("Save Failed", "Only one node should have no input connection", "OK");
            return;
        }

        DialogueNode node = portsNotConnected[0].node as DialogueNode;

        startNode = new DialogueNodeData
        {
            Guid = node.GUID,
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
            DialogueNode outputNode = (connectedPorts[i].output.node as DialogueNode);
            DialogueNode inputNode = (connectedPorts[i].input.node as DialogueNode);
                
            // Create connection data
            nodeLinks.Add(new NodeLinkData
            {
                BaseNodeGuid = outputNode.GUID,
                PortName = connectedPorts[i].output.portName,
                TargetNodeGuid = inputNode.GUID,
                ChoiceGUID = connectedPorts[i].output.name,
            });;
        }


        #endregion

        #region Save Nodes

        // Save all nodes
        foreach (DialogueNode dialogueNode in Nodes)
        {

            // Cloning choices so there won't be a reference in scriptable object
            List<DialogueChoices> choices = new List<DialogueChoices>();

            foreach (DialogueChoices choice in dialogueNode.choices)
            {
                DialogueChoices choiceData = new DialogueChoices(choice.text, choice.portGUID);
                choiceData.targetGUID = choice.targetGUID;

                choices.Add(choiceData);
            }

            DialogueNodeData newNode = new DialogueNodeData
            {
                Guid = dialogueNode.GUID,
                DialogueText = dialogueNode.dialogueText,
                Npc = dialogueNode.npcName,
                Position = dialogueNode.GetPosition().position,
                Connections = nodeLinks.Where(x => x.BaseNodeGuid == dialogueNode.GUID).ToList(),
                Choices = choices,
                NodeType = dialogueNode.nodeType,
            };

            // Starting node
            if (dialogueNode.GUID == startNode.Guid)
            {
                newNode.isStartNode = true;
            }

            eventContainer.DialogueNodeData.Add(newNode);
        }

        #endregion

        // If there is no resources folder create one
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
        {
            AssetDatabase.CreateFolder("Assets", "Resources");
        }

        // Check if asset exists
        if (!string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID($"Assets/Resources/{fileName}.asset")))
        {
            // If false cancel
            if (!EditorUtility.DisplayDialog($"Save {fileName}", $"Overwrite {fileName} asset?", "OK", "Cancel"))
            {
                return;
            }
        }



        AssetDatabase.DeleteAsset($"Assets/Resources/{fileName}.asset");
        AssetDatabase.CreateAsset(eventContainer, $"Assets/Resources/{fileName}.asset");
        AssetDatabase.SaveAssets();
    }

    #endregion

    #region LoadGraph

    public void LoadGraph()
    {
        string filePath = EditorUtility.OpenFilePanel("Dialogue Graph", "Assets/Resources", "asset");

        if (!string.IsNullOrEmpty(filePath))
        {
            string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);

            // Check if file exists
            _containerCache = Resources.Load<Event>(fileName);
            if (_containerCache == null)
            {
                EditorUtility.DisplayDialog("File not found", "Target dialogue graph file does not exists!", "OK");
                return;
            }

            ClearGraph();
            CreateNodes();

            ConnectNodes();
        }
    }

    private void ClearGraph()
    {
        // Set entry points guid back from the save. Discard existing guid
        //Nodes.Find(x => x.EntryPoint).GUID = _containerCache.NodeLinks[0].BaseNodeGuid;

        foreach (DialogueNode node in Nodes)
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
            DialogueNode tempNode;

            if (nodeData.NodeType == "Battle Node" || nodeData.NodeType == "End Node")
            {
                tempNode = new DialogueNode(nodeData.Guid, _targetGraphView, nodeData.NodeType);
                tempNode.DrawUtility(nodeData.Position, _targetGraphView.DefaultNodeSize);
            }
            else if (nodeData.Choices.Count > 0)
            {
                tempNode = new DialogueNode(nodeData.Guid, nodeData.Npc, _targetGraphView, nodeData.NodeType, true);
                tempNode.choices = nodeData.Choices;
                tempNode.dialogueText = nodeData.DialogueText;
                tempNode.Draw(nodeData.Position, _targetGraphView.DefaultNodeSize);
            }
            else
            {
                tempNode = new DialogueNode(nodeData.Guid, nodeData.Npc, _targetGraphView, nodeData.NodeType);
                tempNode.dialogueText = nodeData.DialogueText;
                tempNode.Draw(nodeData.Position, _targetGraphView.DefaultNodeSize);
            }
            

            _targetGraphView.AddElement(tempNode);
        }
    }

    private void ConnectNodes()
    {
        // Loop through nodes on graph
        for (int i = 0; i < Nodes.Count; i++)
        {
            // Get all output connections connected to this node
            List<NodeLinkData> connections = _containerCache.DialogueNodeData.First(x => x.Guid == Nodes[i].GUID).Connections;
            for (int j = 0; j < connections.Count; j++)
            {
                string targetNodeGuid = connections[j].TargetNodeGuid;
                DialogueNode targetNode = Nodes.First(x => x.GUID == targetNodeGuid);

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