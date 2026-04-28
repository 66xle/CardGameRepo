using System;
using System.Collections.Generic;

public class CardRuntime
{
    public GearRuntime GearRuntime { get; private set; }
    public Card Card { get; private set; }

    public List<AnimationWrapper> AnimationList { get; private set; }


    /// For Player card runtime, where there is a gear runtime to reference for damage calculation
    public CardRuntime(GearRuntime gearRuntime, CardAnimationData data, float attack, float defence, float blockScale, float health)
    {
        Card card = data.Card;
        
        Card = CopyCard(card);
        AnimationList = data.AnimationList;
        GearRuntime = gearRuntime;

        Card.DisplayDescription = GenerateDescriptionWithDamage(card, gearRuntime.CurrentValue, attack, defence, blockScale, health);
    }

    /// For Enemy card runtime, where there is no gear runtime
    public CardRuntime(CardAnimationData data, float attack, float defence, float blockScale, float health)
    {
        Card card = data.Card;
        Card = CopyCard(card);
        AnimationList = data.AnimationList;

        Card.DisplayDescription = GenerateDescriptionWithDamage(card, 0, attack, defence, blockScale, health);
    }

    public Card CopyCard(Card card)
    {
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

        return copyCard;
    }

    public string GenerateDescriptionWithDamage(Card card, int gearValue, float attack, float defence, float blockScale, float health, Avatar enemy = null, Avatar player = null)
    {
        int weaponAttack = gearValue;

        string displayDescription = card.LinkDescription;

        for (int i = 0; i < card.ValuesToReference.Count; i++)
        {
            float type = card.ValuesToReference[i].x;
            float value = card.ValuesToReference[i].y;

            if (type == 1)
            {
                value = CalculateDamage.GetBlock(defence, value, blockScale);
            }
            else if (type == 2)
            {
                value = CalculateDamage.GetHealAmount(health, value);
            }
            else if (type == 0)
            {
                value = CalculateDamage.GetDamage(attack, weaponAttack, enemy, player, value);
            }

            displayDescription = displayDescription.Replace($"#{i}", $"<color=#FF0000>{value.ToString()}</color>");
        }

        return displayDescription;
    }

}
