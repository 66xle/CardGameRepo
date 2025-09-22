using System;
using System.Collections.Generic;
using MyBox;
using PixelCrushers.DialogueSystem;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public struct TutorialData
{
    [ConversationPopup(true)] public string ConversationTitle;
}


public class CombatUIManager : MonoBehaviour
{
    [Foldout("Tutorial", true)]
    [MustBeAssigned] [SerializeField] TutorialUI TutorialUI;
    [MustBeAssigned][SerializeField] DialogueSystemSceneEvents events;
    [MustBeAssigned][SerializeField] DialogueDatabase DialogueDatabase;
    [SerializeField] List<TutorialData> TutorialDatas;
    private int _tutorialIndex;
    private DialogueEntry _currentEntry;
    private Conversation _currentConversation;

    [Foldout("Popup", true)]
    [MustBeAssigned] public GameObject DamagePopupPrefab;
    [MustBeAssigned] public Transform WorldSpaceCanvas;
    [Range(0, 1)] public float RandomOffsetHorizontal = 0.5f;
    public float OffsetVertical = 1f;
    public float baseScale = 1f;

    [Separator("Animate")]
    public float MoveDuration;
    public float FadeDuration;
    public float MoveVertical;

    [Foldout("Button", true)]
    [MustBeAssigned] public Button EndTurnButton;
    [MustBeAssigned] public Button SwitchButton;

    [Foldout("Turns", true)]
    public float TurnDuration = 1f;
    public float TurnFadeDuration = 1f;
    [MustBeAssigned] public GameObject PlayerTurnUI;
    [MustBeAssigned] public GameObject EnemyTurnUI;

    [Foldout("Player UI", true)]
    [MustBeAssigned] public Slider HealthBar;
    [MustBeAssigned] public TMP_Text HealthValue;
    [MustBeAssigned] public Slider StaminaBar;
    [MustBeAssigned] public TMP_Text StaminaValue;
    [MustBeAssigned] public Slider GuardBar;
    [MustBeAssigned] public TMP_Text GuardValue;
    [MustBeAssigned] public TMP_Text BlockValue;
    [MustBeAssigned] public GameObject PlayerUI;

    [Foldout("Enemy", true)]
    [MustBeAssigned] public GameObject enemyUIPrefab;
    [MustBeAssigned] public DetailedUI detailedUI;

    [Foldout("UI", true)]
    [MustBeAssigned] public GameObject CombatUI;
    [MustBeAssigned] public GameObject SwitchWeaponUI;
    [MustBeAssigned] public GameObject HideUI;
    [MustBeAssigned] public GameObject DetailedUI;

    [Foldout("Managers", true)]
    [MustBeAssigned] public StatsManager StatsManager;

    public void HideGameplayUI(bool toggle)
    {
        toggle = !toggle;

        HideUI.SetActive(toggle);
        DetailedUI.SetActive(toggle);
        PlayerUI.SetActive(toggle);
    }

    public void InitPlayerUI(Player player)
    {
        player.InitUI(HealthBar, HealthValue, StaminaBar, StaminaValue, BlockValue, GuardBar, GuardValue, StatsManager.ArmourType);
        player.InitStats(StatsManager.CurrentMaxHealth, StatsManager.CurrentMaxStamina, StatsManager.CurrentMaxGuard, StatsManager.Defence, StatsManager.DefencePercentage, StatsManager.Attack, StatsManager.BlockScale);
    }

    public void InitTutorial()
    {
        if (GameManager.Instance.IsInTutorial)
        {
            _tutorialIndex = -1;

            StartTutorialConversation(0);
        }
    }

    public void StartTutorialConversation(int index)
    {
        _currentConversation = DialogueDatabase.GetConversation(TutorialDatas[index].ConversationTitle);
        _currentEntry = _currentConversation.GetFirstDialogueEntry();

        DisplayNextTutorial();
    }

    public void DisplayNextTutorial()
    {
        // Confirm button
        if (_currentEntry.outgoingLinks.Count == 0)
        {
            TutorialUI.CloseTutorial();
            return;
        }

        // Grab next entry
        int destinationID = _currentEntry.outgoingLinks[0].destinationDialogueID;
        _currentEntry = _currentConversation.GetDialogueEntry(destinationID);

        // Grab image
        string imagePath = _currentEntry.fields[1].value;
        Sprite sprite = Resources.Load<Sprite>(imagePath);

        // Last node
        if (_currentEntry.outgoingLinks.Count == 0)
        {
            TutorialUI.DisplayTutorial(sprite, _currentEntry.MenuText, _currentEntry.DialogueText, "Confirm");
            return;
        }
            

        TutorialUI.DisplayTutorial(sprite, _currentEntry.MenuText, _currentEntry.DialogueText);
    }
}
