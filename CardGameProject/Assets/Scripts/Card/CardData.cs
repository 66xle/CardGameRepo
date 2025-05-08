using System;

public struct CardData
{
    public WeaponData Weapon { get; private set; }
    public Card Card { get; private set; }

    public CardData(WeaponData weapon, Card card)
    {
        Weapon = weapon;

        Card copyCard = new Card();
        copyCard.Guid = card.Guid;
        copyCard.InGameGUID = Guid.NewGuid().ToString();
        copyCard.CardName = card.CardName;
        copyCard.Description = card.Description;
        copyCard.DisplayDescription = card.DisplayDescription;
        copyCard.Flavour = card.Flavour;
        copyCard.ValuesToReference = card.ValuesToReference;
        copyCard.Cost = card.Cost;
        copyCard.RecycleValue = card.RecycleValue;
        copyCard.Image = card.Image;
        copyCard.Frame = card.Frame;
        copyCard.CardName = card.CardName;
        copyCard.Commands = card.Commands;

        Card = copyCard;
    }
}