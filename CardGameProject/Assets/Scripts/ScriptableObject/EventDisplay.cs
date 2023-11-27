using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class EventDisplay : MonoBehaviour
{
    private const string DIALOGUE = "Dialogue";
    private const string DIALOGUE_CHOICE = "Dialogue Choice";
    private const string BATTLENODE = "Battle Node";

    [Header("References")]
    public Image image;
    public TextMeshProUGUI dialogueText;
    public GameObject choicePrefab;
    public Transform parentChoiceObject;
    public GridMap gridMap;
    public CombatStateMachine turnBaseSystem;
    public GameObject mapScene;
    public GameObject combatScene;

    private List<DialogueNodeData> dialogueData;
    private List<EnemyObj> enemyList;
    private DialogueNodeData currentNode;
    private bool doesNodeHaveChoice = false;
    [HideInInspector] public bool waitToContinueDialogue = false;
    private List<DialogueChoices> currentChoices = new List<DialogueChoices>();

    

    private void Start()
    {
        waitToContinueDialogue = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && waitToContinueDialogue)
        {
            waitToContinueDialogue = false;
            NextDialogue();
        }

        Debug.Log(doesNodeHaveChoice);
    }

    public void Display(Event eventObj)
    {
        gameObject.SetActive(true);

        image.sprite = eventObj.image;
        dialogueData = eventObj.DialogueNodeData;
        enemyList = eventObj.enemyList;


        currentNode = dialogueData.First(x => x.isStartNode == true);

        DetermineNodeType();
    }

    void DetermineNodeType()
    {
        if (currentNode.NodeType == DIALOGUE || currentNode.NodeType == DIALOGUE_CHOICE)
        {
            LoadDialogue();
        }
        else if (currentNode.NodeType == BATTLENODE)
        {
            LoadCombatEvent();
        }
        else
        {
            EndEvent();
        }
    }

    void LoadDialogue()
    {
        doesNodeHaveChoice = false;

        if (currentNode.Choices.Count == 0)
            waitToContinueDialogue = true;

        ClearChoices();

        dialogueText.text = currentNode.DialogueText;

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


            newChoiceUI.GetComponentInChildren<TextMeshProUGUI>().text = choice.text;

            newChoiceUI.GetComponent<Button>().onClick.AddListener(delegate { NextDialogue(choice); });

            index++;
            currentChoices.Add(choice);
        }
    }

    public void NextDialogue(DialogueChoices index = null)
    {
        DialogueNodeData nextNode;

        if (doesNodeHaveChoice)
        {
            // Get next node through choice picked
            DialogueChoices nextChoice = currentChoices.First(x => x == index);
            nextNode = dialogueData.First(x => x.Guid == nextChoice.targetGUID);
        }
        else
        {
            // Get next node through connection
            string targetNodeGuid = currentNode.Connections[0].TargetNodeGuid;
            nextNode = dialogueData.First(x => x.Guid == targetNodeGuid);
        }

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
        gameObject.SetActive(false);

        gridMap.disableTileInteract = false;
    }

    void LoadCombatEvent()
    {
        Debug.Log("yes");

        turnBaseSystem.enemyList.Clear();
        turnBaseSystem.enemyList = enemyList;


        combatScene.SetActive(true);
        mapScene.SetActive(false);
    }    
}
