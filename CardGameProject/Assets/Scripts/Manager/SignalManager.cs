using UnityEngine;
using UnityEngine.Timeline;

public class SignalManager : MonoBehaviour
{
    public SignalAsset Signal;


    public void Update()
    {
        if (!GameManager.Instance.SkipCutscene) return;


        if (Input.GetKeyDown(KeyCode.Space))
        {
            SendSignalToCutsceneManager();
        }
    }

    public void SendSignalToCutsceneManager()
    {
        CutsceneManager cutscene = ServiceLocator.Get<CutsceneManager>();
        cutscene.PlaySignal(Signal);
    }
}
