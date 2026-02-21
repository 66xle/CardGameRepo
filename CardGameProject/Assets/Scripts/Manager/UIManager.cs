using MyBox;
using Systems.SceneManagment;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] DifficultyManager DifficultyManager;
    [SerializeField] TutorialManager TutorialManager;
    [SerializeField] CombatStateMachine Ctx;
    [SerializeField] GameObject OptionMenu;


    public void RestartCombat(GameObject menu)
    {
        menu.SetActive(false);

        //AudioManager.Instance.FadeOutMusic(0.2f);

        if (TutorialManager.TutorialStage < 5)
            TutorialManager.TutorialStage = 1;

        Ctx.EndGameplay();
        Ctx.player.ResetDeath();
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

    public void EnableOptionMenu(bool toggle)
    {
        OptionMenu.SetActive(toggle);
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
