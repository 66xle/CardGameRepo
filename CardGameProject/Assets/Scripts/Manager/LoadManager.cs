using System.Collections;
using Systems.SceneManagment;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class LoadManager : MonoBehaviour
{
    [SerializeField] AudioResource Resource;

    private void Awake()
    {
        AudioManager.Instance.PlayMusic(Resource);
    }

    public void CloseLoadingScreenSignal()
    {
        AudioManager.Instance.FadeOutMusic(0f);

        SceneLoader loader = ServiceLocator.Get<SceneLoader>();
        loader.CloseLoadingScreen();
    }
}
