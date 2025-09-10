using System.Collections;
using Systems.SceneManagment;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadManager : MonoBehaviour
{
    public void CloseLoadingScreenSignal()
    {
        SceneLoader loader = ServiceLocator.Get<SceneLoader>();
        loader.CloseLoadingScreen();
    }
}
