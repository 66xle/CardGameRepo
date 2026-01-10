using MyBox;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public bool IsInTutorial = false;
    [MustBeAssigned] [SerializeField] CombatUIManager CombatUIManager;



    private float _tutorialStage = 1f;

    private void OnEnable()
    {
        Bus<EventCardPlayed>.OnEvent += OnCardPlayed;
        Bus<EventEndTurn>.OnEvent += OnEndTurn;
        Bus<EventPlayState>.OnEvent += OnPlayState;
    }

    private void OnDisable()
    {
        Bus<EventCardPlayed>.OnEvent -= OnCardPlayed;
        Bus<EventEndTurn>.OnEvent -= OnEndTurn;
        Bus<EventPlayState>.OnEvent -= OnPlayState;
    }


    private void OnCardPlayed(EventCardPlayed eventOnCardPlayed)
    {
        if (!IsInTutorial) return;

        if (_tutorialStage >= 1 && _tutorialStage < 1.2f)
        {
            _tutorialStage += 0.1f;
        }
    }

    private void OnEndTurn(EventEndTurn eventEndTurn)
    {
        if (!IsInTutorial) return;

        if (_tutorialStage == 1.2f)
            _tutorialStage = 2;
        else if (_tutorialStage == 2.1f)
            _tutorialStage = 3;
        else if (_tutorialStage == 3.1f)
            _tutorialStage = 4;
    }

    private void OnPlayState(EventPlayState eventPlayState)
    {
        if (!IsInTutorial) return;

        if (_tutorialStage == 1)
        {
            CombatUIManager.InitTutorial();
            CombatUIManager.EndTurnButton.interactable = false;
        }

        if (_tutorialStage >= 1.2f)
            CombatUIManager.EndTurnButton.interactable = true;

        if (_tutorialStage >= 1.2f && _tutorialStage < 2)
        {
            CombatUIManager.StartTutorialConversation(1);
        }
        else if (_tutorialStage == 2)
        { // Play a card
            CombatUIManager.StartTutorialConversation(2);
            _tutorialStage = 2.1f;
        }
        else if (_tutorialStage == 3)
        {
            CombatUIManager.StartTutorialConversation(3);
            _tutorialStage = 3.1f;
        }
        else if (_tutorialStage == 4)
        {
            CombatUIManager.StartTutorialConversation(4);
            _tutorialStage = 5;
        }
    }
}
