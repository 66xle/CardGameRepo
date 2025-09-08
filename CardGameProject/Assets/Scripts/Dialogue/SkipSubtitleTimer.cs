using PixelCrushers.DialogueSystem;
using UnityEngine;

public class SkipSubtitleTimer : MonoBehaviour
{
    public TextMeshProTypewriterEffect TypewriterEffect;
    public StandardDialogueUI StandardUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InstantShowResponse()
    {
        Response[] responses = DialogueManager.currentConversationState.pcResponses;
        Subtitle subtitle = DialogueManager.currentConversationState.subtitle;

        if (TypewriterEffect.isPlaying)
        {
            if (responses.Length > 0)
            {
                StandardUI.ShowResponses(subtitle, responses, 0f);
                return;
            }

            StandardUI.ShowSubtitle(subtitle);
            StandardUI.ShowContinueButton(subtitle);
        }
    }   
}
