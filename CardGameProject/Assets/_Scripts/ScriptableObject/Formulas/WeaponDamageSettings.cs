using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Damage Settings", menuName = "Weapon Damage Settings")]
public class WeaponDamageSettings : ScriptableObject
{
    [Header("Damage Formula")]
    [SerializeField] float A = 2;
    [SerializeField] float B = 2;
    [SerializeField] float BaseAtk = 1;

    [Header("Rarity")]
    [SerializeField] float Common = 1;
    [SerializeField] float Rare = 2;
    [SerializeField] float Epic = 3;
    [SerializeField] float Legendary = 4;

    [Header("Type")]
    [SerializeField] float Sword = 1;
    [SerializeField] float Dagger = 0.8f;
    [SerializeField] float Club = 1.2f;
    [SerializeField] float TwoHanded = 1.4f;


    public int GetWeaponDamage(WeaponData data)
    {
        float level = GetRarity(data.Rarity);

        float a = A * level;
        float b = B * level;
        float c = BaseAtk;

        return Mathf.RoundToInt(Mathf.Pow(a, 2) + b + c);
    }

    public float GetRarity(Rarity rarity)
    {
        if (rarity == Rarity.Rare)
            return Rare;
        else if (rarity == Rarity.Epic)
            return Epic;
        else if (rarity == Rarity.Legendary)
            return Legendary;

        return Common;
    }
}
