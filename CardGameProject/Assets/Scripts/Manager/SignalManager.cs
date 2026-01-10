using Systems.SceneManagment;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class SignalManager : MonoBehaviour
{
    public SignalAsset Signal;
    public SignalAsset EndlessSignal;
    public SignalAsset ResumeSignal;

    public AudioResource Resource;


    public void SendSignalToCutsceneManager()
    {
        CutsceneManager cutscene = ServiceLocator.Get<CutsceneManager>();
        cutscene.PlaySignal(Signal);
    }

    public void SendEndlessSignal()
    {
        CutsceneManager cutscene = ServiceLocator.Get<CutsceneManager>();
        cutscene.PlaySignal(EndlessSignal);
    }

    public void SendResumeSignal()
    {
        CutsceneManager cutscene = ServiceLocator.Get<CutsceneManager>();
        cutscene.PlaySignal(ResumeSignal);
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
