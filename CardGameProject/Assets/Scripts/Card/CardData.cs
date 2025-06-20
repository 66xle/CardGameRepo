using System;
using System.Collections.Generic;
using UnityEngine;

public struct CardData
{
    public WeaponData Weapon { get; private set; }
    public Card Card { get; private set; }

    public List<AnimationWrapper> AnimationList { get; private set; }

    public CardData(WeaponData weapon, WeaponCardData data)
    {
        Card card = data.Card;
        Card copyCard = new Card();

        copyCard.Guid = card.Guid;
        copyCard.InGameGUID = Guid.NewGuid().ToString();

        copyCard.CardName = card.CardName;
        copyCard.Description = card.Description;
        copyCard.DisplayDescription = card.DisplayDescription;
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
    }

}