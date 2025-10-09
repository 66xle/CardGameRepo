using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Timeline;

public class SignalManager : MonoBehaviour
{
    public SignalAsset Signal;
    public AudioResource Resource;


    public void Update()
    {
        if (!GameManager.Instance.SkipCutscene) return;

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SendSignalToCutsceneManager();
        }
#endif
    }

    public void SendSignalToCutsceneManager()
    {
        CutsceneManager cutscene = ServiceLocator.Get<CutsceneManager>();
        cutscene.PlaySignal(Signal);
    }

    public void PlayMusic()
    {
        AudioManager.Instance.PlayMusic(Resource);
    }
}
