using System.Collections.Generic;
using MyBox;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

public enum AudioType
{
    None,
    UIClick,
    UIGameOver,
    CardPlay,
    CardDiscard,
    CardDraw,
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

    public new void Awake()
    {
        base.Awake();
        transform.SetParent(null);
        DontDestroyOnLoad(this);
    }

    public void PlaySound(AudioType audioType)
    {
        AudioData data = null;

        foreach (AudioWrapper wrapper in AudioWrappers)
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
        musicSource.resource = data.audios[Random.Range(0, data.audios.Count)];
        musicSource.Play();
    }
}
