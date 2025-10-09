using Systems.SceneManagment;
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

    public async void MainMenu()
    {
        Time.timeScale = 1f;

        AudioManager.Instance.FadeOutMusic(0.2f);

        SceneLoader sceneLoader = ServiceLocator.Get<SceneLoader>();
        await sceneLoader.LoadSceneGroup("MainMenu");
    }
}
