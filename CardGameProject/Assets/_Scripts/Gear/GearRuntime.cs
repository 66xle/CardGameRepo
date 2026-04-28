using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

public class GearRuntime
{
    public GearData GearData { get; private set; }


    public int Level;
    public int CurrentValue;
    public int BaseValue { get; private set; }

    public List<CardRuntime> CardRuntimes;



    public GearRuntime(GearData gear, StatsManager statsManager)
    {
        if (gear == null) return;

        GearData = gear;

        Level = 1;
        BaseValue = gear.Value;
        CurrentValue = BaseValue;
        CardRuntimes = new();

        foreach (CardAnimationData cardData in gear.Cards)
        {
            for (int i = 0; i < cardData.CardAmount; i++)
            {
                CardRuntime cardRuntime = new(this, cardData, statsManager.Attack, statsManager.Defence, statsManager.BlockScale, statsManager.CurrentMaxHealth);
                CardRuntimes.Add(cardRuntime);
            }
        }
    }


    public bool IsWeapon()
    {
        return GearData is WeaponData;
    }

    public bool IsArmour()
    {
        return GearData is ArmourData;
    }




}
