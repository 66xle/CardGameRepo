using System;

public struct CardData
{
    public WeaponData Weapon { get; private set; }
    public Card Card { get; private set; }

    public CardData(WeaponData weapon, Card card)
    {
        Weapon = weapon;

        Card copyCard = new Card();
        copyCard.guid = card.guid;
        copyCard.InGameGUID = Guid.NewGuid().ToString();
        copyCard.cardName = card.cardName;
        copyCard.description = card.description;
        copyCard.displayDescription = card.displayDescription;
        copyCard.flavour = card.flavour;
        copyCard.valuesToReference = card.valuesToReference;
        copyCard.cost = card.cost;
        copyCard.recycleValue = card.recycleValue;
        copyCard.image = card.image;
        copyCard.frame = card.frame;
        copyCard.cardName = card.cardName;
        copyCard.commands = card.commands;

        Card = copyCard;
    }
}