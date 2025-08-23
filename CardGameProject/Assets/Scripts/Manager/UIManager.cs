using MyBox;
using Systems.SceneManagment;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [MustBeAssigned] [SerializeField] DifficultyManager DifficultyManager;

    public async void RestartCombat()
    {
        Time.timeScale = 1.0f;

        SceneLoader loader = ServiceLocator.Get<SceneLoader>();

        await loader.LoadSceneGroup("Combat");
    }

    public void NextScene()
    {
        // difficulty manager
        DifficultyManager.OnBattleComplete();

        GameManager.Instance.StageLevel++;

        RestartCombat(); // temp
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
