using System.Collections.Generic;
using MyBox;
using UnityEngine;
using UnityEngine.Audio;

public enum SoundType
{
    UIClick
}

public class AudioManager : Singleton<AudioManager>
{
    [Foldout("UI", true)]
    public AudioData UIClick;

    AudioSource audioSource;

    public void Start()
    {
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(SoundType soundType)
    {
        AudioData data = null;

        if (SoundType.UIClick == soundType)
        {
            data = UIClick;
        }

        if (data == null) return;

        audioSource.resource = data.audios[Random.Range(0, data.audios.Count)];

        audioSource.Play();
    }

    public void PlaySoundButton(AudioData selectedData)
    {
        AudioData data = selectedData;

        audioSource.resource = data.audios[Random.Range(0, data.audios.Count)];

        audioSource.Play();
    }
}
