using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class OptionUI : MonoBehaviour
{
    [SerializeField] TMP_Dropdown resolutionDropdown;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetupResolution();
    }

    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions;

    private RefreshRate currentRefreshRate;
    private int currentResolutionIndex = 0;

    private void SetupResolution()
    {
        resolutions = Screen.resolutions;
        filteredResolutions = new List<Resolution>();

        resolutionDropdown.ClearOptions();
        currentRefreshRate = Screen.currentResolution.refreshRateRatio;

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].refreshRateRatio.ToString() == currentRefreshRate.ToString())
            {
                filteredResolutions.Add(resolutions[i]);
            }
        }

        List<string> options = new List<string>();

        for (int i = filteredResolutions.Count - 1; i >= 0; i--)
        {
            string resolutionOption = filteredResolutions[i].width + " x " + filteredResolutions[i].height;
            options.Add(resolutionOption);

            if (filteredResolutions[i].width == Screen.width && filteredResolutions[i].height == Screen.height)
            {
                currentResolutionIndex = 0;
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
    }
}
