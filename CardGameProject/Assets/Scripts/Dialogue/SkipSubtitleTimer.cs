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
        if (TypewriterEffect.isPlaying)
        {
            StandardUI.ShowResponses(DialogueManager.currentConversationState.subtitle, DialogueManager.currentConversationState.pcResponses, 0f);
        }
    }   
}
