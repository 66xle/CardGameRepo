using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    


    public void RestartCombat()
    {
        Time.timeScale = 1.0f;

        GameManager.SceneToLoad = SceneManager.GetActiveScene().name;

        SceneManager.LoadSceneAsync("LoadingScene");
    }
}
