using UnityEngine;

public class MainMenuManager : MonoBehaviour
{

    [SerializeField] AudioSource ElevatorMusic;
    [SerializeField] AudioSource MainMenuMusic;

    private void Awake()
    {
        SceneInitialize.Instance.Subscribe(Init, -10);
    }

    private void Init()
    {
        ElevatorMusic.Play();
        MainMenuMusic.Play();
    }
}
