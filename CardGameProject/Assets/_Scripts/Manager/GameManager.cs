using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [HideInInspector] public string SceneToLoad;
    [HideInInspector] public int StageLevel;

    [HideInInspector] public GearRuntime MainHand;
    [HideInInspector] public GearRuntime OffHand;
    [HideInInspector] public List<GearRuntime> EquippedWeapons;
    [HideInInspector] public List<GearRuntime> EquippedArmour;
    [HideInInspector] public bool IsEquipmentSaved = false;


    [HideInInspector] public int DifficultyScore;
    [HideInInspector] public int WaveCount;

    [HideInInspector] public int PlayerLevel;
    [HideInInspector] public int CurrentEXP;


    [HideInInspector] public LevelData CurrentLevelDataLoaded;
    [HideInInspector] public GameObject LoadedEnvironment;

    public bool IsInTutorial = true;
    [HideInInspector] public float TutorialStage = 1;

    // Launch
    public bool HasOptionLoadedThisSession = false;

    public new void Awake()
    {
        base.Awake();
        transform.SetParent(null);
        DontDestroyOnLoad(this);
    }
}
