using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public struct CardData
{
    public WeaponData Weapon { get; private set; }
    public Card Card { get; private set; }

    public List<AnimationWrapper> AnimationList { get; private set; }

    public CardData(WeaponData weapon, CardAnimationData data, float attack, float defence, float blockScale)
    {
        Card card = data.Card;
        Card copyCard = new Card();

        copyCard.Guid = card.Guid;
        copyCard.InGameGUID = Guid.NewGuid().ToString();

        copyCard.CardName = card.CardName;
        copyCard.Description = card.Description;
        copyCard.LinkDescription = card.LinkDescription;
        copyCard.Flavour = card.Flavour;
        copyCard.PopupKeyPair = card.PopupKeyPair;

        copyCard.Cost = card.Cost;
        copyCard.RecycleValue = card.RecycleValue;

        copyCard.Image = card.Image;
        copyCard.Frame = card.Frame;

        copyCard.ValuesToReference = card.ValuesToReference;
        copyCard.Commands = card.Commands;


        AnimationList = data.AnimationList;
        Card = copyCard;
        Weapon = weapon;

        Card.DisplayDescription = GenerateDescriptionWithDamage(card, weapon, attack, defence, blockScale);
    }

    public string GenerateDescriptionWithDamage(Card card, WeaponData weapon, float attack, float defence, float blockScale, Avatar enemy = null)
    {
        string displayDescription = card.LinkDescription;

        for (int i = 0; i < card.ValuesToReference.Count; i++)
        {
            float type = card.ValuesToReference[i].x;
            float value = card.ValuesToReference[i].y;

            if (type == 1)
            {
                value = CalculateDamage.GetBlock(defence, value, blockScale);
            }
            else
            {
                value = CalculateDamage.GetDamage(attack, weapon.WeaponAttack, enemy, value);
            }

            displayDescription = displayDescription.Replace($"#{i}", $"<color=#FF0000>{value.ToString()}</color>");
        }

        return displayDescription;
    }

}