using MyBox;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [MustBeAssigned] [SerializeField] AudioData MainMenuMusic;

    private void Awake()
    {
        //AudioManager.Instance.PlayMusic(MainMenuMusic);
    }

    public void FadeOut(float duration)
    {
        AudioManager.Instance.FadeOutMusic(duration);
    }
}