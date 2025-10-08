using System.Collections.Generic;
using DG.Tweening;
using MyBox;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Timeline;

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
    [Header("AudioSource Pool Settings")]
    public int poolSize = 10; // number of AudioSources for overlapping sounds
    private AudioSource[] sfxSources;
    private int currentIndex = 0;

    [Foldout("UI", true)]
    public List<AudioWrapper> AudioWrappers;
    public List<AudioWrapper> AttackSounds;

    [Foldout("Audio Sources", true)]
    [MustBeAssigned][SerializeField] AudioSource audioSource;
    [MustBeAssigned][SerializeField] AudioSource musicSource;

    private List<AudioWrapper> wrappers = new();

    private AudioResource _type;
    public new void Awake()
    {
        base.Awake();
        transform.SetParent(null);
        DontDestroyOnLoad(this);

        // Initialize the AudioSource pool
        sfxSources = new AudioSource[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            GameObject go = new GameObject("SFXSource_" + i);
            go.transform.SetParent(transform);
            AudioSource source = go.AddComponent<AudioSource>();
            source.playOnAwake = false;
            sfxSources[i] = source;
        }

        wrappers.AddRange(AudioWrappers);
        wrappers.AddRange(AttackSounds);
    }

    #region Animations
    public void SetAudioResource(AudioResource resource)
    {
        audioSource.resource = resource;
    }

    public void PlayAudioResource()
    {
        audioSource.Play();
    }
    #endregion


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

        AudioResource resource = data.audios[Random.Range(0, data.audios.Count)];
        PlaySound(resource);
    }
    
    public void PlaySound(AudioData data)
    {
        audioSource.resource = data.audios[Random.Range(0, data.audios.Count)];
        audioSource.PlayOneShot(audioSource.clip);
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

    /// <summary>
    /// Play an AudioResource using pooled AudioSources
    /// </summary>
    public void PlaySound(AudioResource resource)
    {
        if (resource == null) return;

        AudioSource source = sfxSources[currentIndex];

        source.resource = resource;
        source.Play();

        // Move to next AudioSource in the pool (round-robin)
        currentIndex = (currentIndex + 1) % poolSize;
    }

    /// <summary>
    /// Optional: Play a raw AudioClip directly
    /// </summary>
    public void PlaySound(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        if (clip == null) return;

        AudioSource source = sfxSources[currentIndex];

        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.Play();

        currentIndex = (currentIndex + 1) % poolSize;
    }
}
