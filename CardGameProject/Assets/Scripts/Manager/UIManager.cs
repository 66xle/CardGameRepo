using MyBox;
using Systems.SceneManagment;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [MustBeAssigned] [SerializeField] DifficultyManager DifficultyManager;


    public void RestartCombat()
    {
        Time.timeScale = 1.0f;

        GameManager.Instance.SceneToLoad = SceneManager.GetActiveScene().name;

        SceneManager.LoadSceneAsync("LoadingScene");
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
