using System.Collections.Generic;
using MyBox;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

public enum AudioType
{
    None,
    UIClick
}

public class AudioManager : Singleton<AudioManager>
{
    [Foldout("UI", true)]
    public AudioData UIClick;

    private AudioSource audioSource;

    public void Start()
    {
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioType audioType)
    {
        AudioData data = null;

        if (AudioType.UIClick == audioType)
        {
            data = UIClick;
        }

        if (data == null)
        {
            Debug.LogError("audioType is null");
            return;
        }

        audioSource.resource = data.audios[Random.Range(0, data.audios.Count)];
        audioSource.Play();
    }
}
