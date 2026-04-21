using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using MyBox;
using SerializeReferenceEditor;
using UnityEngine;

[Serializable]
public class CardVariant
{
    public string Name;

    [Separator]

    public bool OverrideDescription;
    public bool OverrideFlavour;
    public bool OverrideImage;
    public bool OverrideFrame;
    public bool OverrideCost;
    public bool OverrideRecycleValue;
    public bool OverrideCommands;


    // Not shown in inspector (Editor creates these fields)
    public string Description = string.Empty;
    public string Flavour = string.Empty;



    [ReadOnly(nameof(OverrideImage), true)] public Sprite Image;
    [ReadOnly(nameof(OverrideFrame), true)] public Sprite Frame;

    [ReadOnly(nameof(OverrideCost), true)] public int Cost = 0;
    [ReadOnly(nameof(OverrideRecycleValue), true)] public int RecycleValue = 0;

    [Separator]

    [SerializeReference][SR] [ReadOnly(nameof(OverrideCommands), true)] public List<Executable> Commands;

    public CardVariant(Card card)
    {
       Description = card.Description;
       Flavour = card.Flavour;
       Image = card.Image;
       Frame = card.Frame;
       Cost = card.Cost;
       RecycleValue = card.RecycleValue;
       Commands = card.Commands.Select(c => c.Clone()).ToList();
    }



}
