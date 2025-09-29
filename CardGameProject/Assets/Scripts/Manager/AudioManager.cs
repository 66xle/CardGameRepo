using System.Collections.Generic;
using DG.Tweening;
using MyBox;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

public enum AudioType
{
    None = 0,
    UIClick = 1,
    UIConfirm = 2,
    UIReturn = 3,
    UIGameOver = 4,
    UIReward = 5,
    UIViewGear = 6,
    UISelectGear = 7,
    UIPlayerTurn = 8,
    UIEnemyTurn = 9,
    UIEndTurn = 10,
    CardPlay= 100,
    CardDiscard = 101,
    CardDraw = 102,
    OneHandSword = 200,
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
    public List<AudioWrapper> AttackSounds;

    [Foldout("Audio Sources", true)]
    [MustBeAssigned][SerializeField] AudioSource audioSource;
    [MustBeAssigned][SerializeField] AudioSource musicSource;

    private List<AudioWrapper> wrappers = new();

    private AudioType _type;
    public new void Awake()
    {
        base.Awake();
        transform.SetParent(null);
        DontDestroyOnLoad(this);

        wrappers.AddRange(AudioWrappers);
        wrappers.AddRange(AttackSounds);
    }

    public void SetAudioType(AudioType audioType)
    {
        _type = audioType;
    }

    public void PlayAudioType()
    {
        PlaySound(_type);
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
    
    public void PlaySound(AudioData data)
    {
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
