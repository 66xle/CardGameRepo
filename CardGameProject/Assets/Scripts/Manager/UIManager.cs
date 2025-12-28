using MyBox;
using Systems.SceneManagment;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [MustBeAssigned] [SerializeField] DifficultyManager DifficultyManager;
    [MustBeAssigned] [SerializeField] CombatStateMachine Ctx;

    public void RestartCombat(GameObject menu)
    {
        menu.SetActive(false);

        //AudioManager.Instance.FadeOutMusic(0.2f);

        if (GameManager.Instance.TutorialStage < 5)
            GameManager.Instance.TutorialStage = 1;

        Ctx.EndGameplay();
        Ctx.Init();

        Time.timeScale = 1.0f;

        //SceneLoader loader = ServiceLocator.Get<SceneLoader>();

        //await loader.LoadSceneGroup("Combat");
    }

    public async void MainMenu()
    {
        Time.timeScale = 1f;

        GameManager.Instance.WaveCount = 0;
        GameManager.Instance.CurrentEXP = 0;

        AudioManager.Instance.FadeOutMusic(0.2f);

        SceneLoader sceneLoader = ServiceLocator.Get<SceneLoader>();
        await sceneLoader.LoadSceneGroup("MainMenu");
    }

    public void NextScene()
    {
        // difficulty manager
        DifficultyManager.OnBattleComplete();

        GameManager.Instance.StageLevel++;

        //RestartCombat(); // temp
    }

    public async void Play()
    {
        SceneLoader sceneLoader = ServiceLocator.Get<SceneLoader>();

        await sceneLoader.LoadSceneGroup("Combat");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
