using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [HideInInspector] public string SceneToLoad;

    [HideInInspector] public WeaponData MainHand;
    [HideInInspector] public WeaponData OffHand;
    [HideInInspector] public List<WeaponData> EquippedWeapons;
    
    [HideInInspector] public bool IsWeaponsSaved = false;



    public new void Awake()
    {
        base.Awake();
        transform.SetParent(null);
        DontDestroyOnLoad(this);
    }
}
