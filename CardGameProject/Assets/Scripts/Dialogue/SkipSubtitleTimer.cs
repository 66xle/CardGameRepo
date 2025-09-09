using PixelCrushers.DialogueSystem;
using UnityEngine;

public class SkipSubtitleTimer : MonoBehaviour
{
    public TextMeshProTypewriterEffect TypewriterEffect;
    public StandardDialogueUI StandardUI;

    public void InstantShowResponse()
    {
        Response[] responses = DialogueManager.currentConversationState.pcResponses;
        Subtitle subtitle = DialogueManager.currentConversationState.subtitle;
        
        if (TypewriterEffect.isPlaying)
        {
            if (responses.Length > 0)
            {
                if (responses[0].destinationEntry.Title == "END") return;

                StandardUI.OnContinueConversation();
            }
        }
    }   
}
