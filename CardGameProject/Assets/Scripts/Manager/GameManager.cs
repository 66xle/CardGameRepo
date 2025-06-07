using System.Collections.Generic;
using MyBox;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [MustBeAssigned] LevelSettings LevelSettings;

    [HideInInspector] public string SceneToLoad;

    [HideInInspector] public WeaponData MainHand;
    [HideInInspector] public WeaponData OffHand;
    [HideInInspector] public List<WeaponData> EquippedWeapons;
    
    [HideInInspector] public bool IsWeaponsSaved = false;

    private int currentLevel;


    public new void Awake()
    {
        base.Awake();
        transform.SetParent(null);
        DontDestroyOnLoad(this);
    }
}
