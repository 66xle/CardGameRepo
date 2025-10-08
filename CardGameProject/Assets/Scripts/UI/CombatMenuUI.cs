using System;
using Systems.SceneManagment;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatMenuUI : MonoBehaviour
{
    public GameObject menu;

    public void OpenMenu()
    {
        menu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void CloseMenu()
    {
        menu.SetActive(false);
        Time.timeScale = 1f;
    }
}
