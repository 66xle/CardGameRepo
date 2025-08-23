using System.Collections.Generic;
using MyBox;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [HideInInspector] public string SceneToLoad;
    [HideInInspector] public int StageLevel;

    [HideInInspector] public WeaponData MainHand;
    [HideInInspector] public WeaponData OffHand;
    [HideInInspector] public List<WeaponData> EquippedWeapons;
    [HideInInspector] public List<ArmourData> EquippedArmour;
    [HideInInspector] public bool IsEquipmentSaved = false;


    [HideInInspector] public int DifficultyScore;

    [HideInInspector] public int PlayerLevel;
    [HideInInspector] public int CurrentEXP;


    [HideInInspector] public LevelData CurrentLevelDataLoaded;

    public new void Awake()
    {
        base.Awake();
        transform.SetParent(null);
        DontDestroyOnLoad(this);
    }
}
