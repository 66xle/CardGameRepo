using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

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

    #region Internal Variables

    private const string DIALOGUE = "Dialogue";
    private const string DIALOGUE_CHOICE = "Dialogue Choice";
    private const string BATTLENODE = "Battle Node";

    private List<DialogueNodeData> dialogueData;
    private List<DialogueChoices> currentChoices;
    private List<EnemyObj> enemyList;
    private DialogueNodeData currentNode;
    private bool doesNodeHaveChoice = false;

    [HideInInspector] public bool waitToContinueDialogue = false;
    [HideInInspector] public bool disableTileInteract = false;

    #endregion


    public void Init()
    {
        dialogueData = new List<DialogueNodeData>();
        currentChoices = new List<DialogueChoices>();
        enemyList = new List<EnemyObj>();

        waitToContinueDialogue = false;
        disableTileInteract = false;
    }

    private void Update()
    {
        if (inputManager.leftClickInputDown && waitToContinueDialogue)
        {
            waitToContinueDialogue = false;
            NextDialogue();
        }
    }

    public void Display(Event eventObj)
    {
        gameObject.SetActive(true);
        Init();

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
        

        if (currentNode.Choices.Count == 0)
            waitToContinueDialogue = true;

        ClearChoices();

        tmpDialougeText.text = currentNode.DialogueText;

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
        gameObject.SetActive(false);
        disableTileInteract = false;
    }

    void LoadCombatEvent()
    {
        combatScene.SetActive(true);
        mapScene.SetActive(false);

        turnBaseSystem.Init(Extensions.Clone(enemyList));
    }    

    public void FinishCombatEvent()
    {
        mapScene.SetActive(true);
        combatScene.SetActive(false);

        NextDialogue();
    }
}
