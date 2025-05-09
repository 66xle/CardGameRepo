using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;


public class EventDisplay : MonoBehaviour
{
    [Header("Component References")]
    public Image image;
    public TextMeshProUGUI tmpDialougeText;

    [Header("References")]
    public GameObject choicePrefab;
    public Transform parentChoiceObject;
    public GameObject mapScene;
    public GameObject combatScene;
    public CombatStateMachine turnBaseSystem;
    public InputManager inputManager;
    public EventManager eventManager; // to access event queue

    #region Internal Variables

    private const string DIALOGUE = "Dialogue";
    private const string DIALOGUE_CHOICE = "Dialogue Choice";
    private const string BATTLENODE = "Battle Node";

    private const string SINGLEEVENT = "Single Event";
    private const string LINKEDEVENT = "Linked Event";

    private List<DialogueNodeData> dialogueData;
    private List<DialogueChoices> currentChoices;
    private DialogueNodeData currentNode;
    private bool doesNodeHaveChoice = false;

    private EventTracker currentEventTracker;
    private Event currentEventObj;

    [HideInInspector] public bool waitToContinueDialogue = false;
    [HideInInspector] public bool disableTileInteract = false;

    #endregion


    public void Init()
    {
        dialogueData = new List<DialogueNodeData>();
        currentChoices = new List<DialogueChoices>();

        waitToContinueDialogue = false;
        disableTileInteract = false;
    }

    private void Update()
    {
        if (inputManager.LeftClickInputDown && waitToContinueDialogue)
        {
            waitToContinueDialogue = false;
            NextDialogue();
        }
    }

    public void Display(EventTracker eventTracker)
    {
        // Saves our event/s
        currentEventTracker = eventTracker;
        currentEventObj = eventTracker.GetEvent();

        gameObject.SetActive(true);
        Init();

        // Get data
        dialogueData = currentEventObj.DialogueNodeData;

        currentNode = dialogueData.First(x => x.isStartNode == true);

        DetermineNodeType();
    }

    void DetermineNodeType()
    {
        if (currentNode == null)
        {
            EndEvent();
        }
        else if (currentNode.NodeType == DIALOGUE || currentNode.NodeType == DIALOGUE_CHOICE)
        {
            LoadDialogue();
        }
        else if (currentNode.NodeType == BATTLENODE)
        {
            LoadCombatEvent();
        }
    }

    void LoadDialogue()
    {
        if (currentNode.Choices.Count == 0)
            waitToContinueDialogue = true;

        ClearChoices();

        // Set text and image
        tmpDialougeText.text = currentNode.DialogueText;
        image.sprite = currentNode.image;


        #region Load Choices

        Transform newChoiceUI = parentChoiceObject.transform;
        int index = 0;

        foreach (DialogueChoices choice in currentNode.Choices)
        {
            doesNodeHaveChoice = true;

            // Create choice ui
            newChoiceUI = Instantiate(choicePrefab, newChoiceUI.transform).transform;
            newChoiceUI.parent = parentChoiceObject.transform;

            // Set button position
            RectTransform rt = newChoiceUI.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(0, rt.anchoredPosition.y - 150f);

            // Set choice text and click event
            newChoiceUI.GetComponentInChildren<TextMeshProUGUI>().text = choice.text;
            newChoiceUI.GetComponent<Button>().onClick.AddListener(delegate { NextDialogue(choice); });

            index++;
            currentChoices.Add(choice);
        }

        #endregion
    }

    public void NextDialogue(DialogueChoices selectedChoice = null)
    {
        DialogueNodeData nextNode = null;
        
        if (doesNodeHaveChoice)
        {
            // Get next node through choice picked
            DialogueChoices nextChoice = currentChoices.First(x => x == selectedChoice);
            nextNode = dialogueData.First(x => x.Guid == nextChoice.targetGUID);
        }
        else
        {
            // Get next node through connection

            if (currentNode.Connections.Count > 0)
            {
                string targetNodeGuid = currentNode.Connections[0].TargetNodeGuid;
                nextNode = dialogueData.First(x => x.Guid == targetNodeGuid);
            }
        }

        doesNodeHaveChoice = false;

        currentNode = nextNode;
        DetermineNodeType();
    }

    void ClearChoices()
    {
        for (int i = 0; i < parentChoiceObject.transform.childCount; i++)
        {
            GameObject child = parentChoiceObject.transform.GetChild(i).gameObject;
            child.GetComponent<Button>().onClick.RemoveAllListeners();

            Destroy(child);
        }
    }

    void EndEvent()
    {
        if (currentEventObj.type == LINKEDEVENT)
        {
            currentEventTracker.EventCompleted();

            if (!currentEventTracker.IsEventFinished())
            {
                eventManager.InsertIntoQueue(currentEventTracker);
            }
        }


        gameObject.SetActive(false);
        disableTileInteract = false;
    }

    void LoadCombatEvent()
    {
        combatScene.SetActive(true);
        mapScene.SetActive(false);

        //turnBaseSystem.Init(currentNode);
    }    

    public void FinishCombatEvent()
    {
        mapScene.SetActive(true);
        combatScene.SetActive(false);

        NextDialogue();
    }
}
