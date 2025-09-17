using System.Collections.Generic;
using DG.Tweening;
using MyBox;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

public enum AudioType
{
    None,
    UIClick,
    UIConfirm,
    UIReturn,
    UIGameOver,
    UIReward,
    UIViewGear,
    UISelectGear,
    UIPlayerTurn,
    UIEnemyTurn,
    UIEndTurn,
    CardPlay,
    CardDiscard,
    CardDraw
}

[System.Serializable]
public class AudioWrapper
{
    public AudioData AudioData;
    public AudioType Type;
}


public class AudioManager : Singleton<AudioManager>
{
    [Foldout("UI", true)]
    public List<AudioWrapper> AudioWrappers;

    [Foldout("Audio Sources", true)]
    [MustBeAssigned][SerializeField] AudioSource audioSource;
    [MustBeAssigned][SerializeField] AudioSource musicSource;

    private List<AudioWrapper> wrappers;

    public new void Awake()
    {
        base.Awake();
        transform.SetParent(null);
        DontDestroyOnLoad(this);

        wrappers.AddRange(AudioWrappers);
    }

    public void PlaySound(AudioType audioType)
    {
        AudioData data = null;

        foreach (AudioWrapper wrapper in wrappers)
        {
            if (wrapper.Type == audioType)
            {
                data = wrapper.AudioData;
            }
        }

        if (data == null)
        {
            Debug.LogError("audioType is null");
            return;
        }

        audioSource.resource = data.audios[Random.Range(0, data.audios.Count)];
        audioSource.Play();
    }

    public void PlayMusic(AudioData data)
    {
        musicSource.volume = 1;

        musicSource.resource = data.audios[Random.Range(0, data.audios.Count)];
        musicSource.Play();
    }

    public void FadeOutMusic(float duration)
    {
        DOVirtual.Float(musicSource.volume, 0, duration, f => musicSource.volume = f).OnComplete(() => musicSource.Stop());
    }
}
