using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using DG.Tweening;
using System.Runtime.CompilerServices;

public class OptionUI : MonoBehaviour
{
    [SerializeField] TMP_Dropdown resolutionDropdown;
    [SerializeField] AudioMixer SoundMixer;
    [SerializeField] Slider MusicSlider;
    [SerializeField] Slider EffectSlider;

    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions;

    private RefreshRate currentRefreshRate;
    private int currentResolutionIndex = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //if (GameManager.Instance.HasOptionLoadedThisSession) return;

        SetupResolution();
        SetupVolume();

        //GameManager.Instance.HasOptionLoadedThisSession = true;
    }

    #region Resolution

    private void SetupResolution()
    {
        resolutions = Screen.resolutions;
        filteredResolutions = new List<Resolution>();

        resolutionDropdown.ClearOptions();
        currentRefreshRate = Screen.currentResolution.refreshRateRatio;

        for (int i = 0; i < resolutions.Length; i++)
        {
            filteredResolutions.Add(resolutions[i]);
        }

        List<string> options = new List<string>();

        for (int i = filteredResolutions.Count - 1; i >= 0; i--)
        {
            string resolutionOption = filteredResolutions[i].width + " x " + filteredResolutions[i].height;
            options.Add(resolutionOption);

            if (filteredResolutions[i].width == Screen.width && filteredResolutions[i].height == Screen.height)
            {
                currentResolutionIndex = PlayerPrefs.GetInt("resolution", 0);
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = filteredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, true);

        PlayerPrefs.SetInt("resolution", resolutionIndex);
    }

    #endregion

    #region Volume

    private void SetupVolume()
    {
        MusicSlider.value = PlayerPrefs.GetFloat("musicVol", 1);
        EffectSlider.value = PlayerPrefs.GetFloat("effectVol", 1);

        SoundMixer.SetFloat("musicVol", Mathf.Log10(MusicSlider.value) * 20);
        SoundMixer.SetFloat("effectVol", Mathf.Log10(EffectSlider.value) * 20);
    }

    public void SetMusicVolume()
    {
        SoundMixer.SetFloat("musicVol", Mathf.Log10(MusicSlider.value) * 20);
        PlayerPrefs.SetFloat("musicVol", MusicSlider.value);
    }

    public void SetEffectVolume()
    {
        SoundMixer.SetFloat("effectVol", Mathf.Log10(EffectSlider.value) * 20);
        PlayerPrefs.SetFloat("effectVol", EffectSlider.value);
    }

    #endregion
}
