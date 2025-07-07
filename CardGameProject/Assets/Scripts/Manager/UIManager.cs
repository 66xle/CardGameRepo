using MyBox;
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

        RestartCombat(); // temp
    }

    public void Play()
    {
        GameManager.Instance.SceneToLoad = "Combat";

        SceneManager.LoadSceneAsync("LoadingScene");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
