using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(AsyncLoadScene(GameManager.Instance.SceneToLoad));
    }

    IEnumerator AsyncLoadScene(string sceneName)
    {
        // temp
        yield return new WaitForSeconds(1f);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

    }
}
