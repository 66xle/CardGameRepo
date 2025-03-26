using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    


    public void RestartCombat()
    {
        Time.timeScale = 1.0f;

        GameManager.sceneToLoad = SceneManager.GetActiveScene().name;

        SceneManager.LoadSceneAsync("LoadingScene");
    }
}
