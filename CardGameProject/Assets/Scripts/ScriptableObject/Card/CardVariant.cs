using System;
using System.Collections.Generic;
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



    [ConditionalField(nameof(OverrideDescription))] public string Description = string.Empty;
    [ConditionalField(nameof(OverrideFlavour))] public string Flavour = string.Empty;

    [ConditionalField(nameof(OverrideImage))] public Sprite Image;
    [ConditionalField(nameof(OverrideFrame))] public Sprite Frame;

    [ConditionalField(nameof(OverrideCost))] public int Cost = 0;
    [ConditionalField(nameof(OverrideRecycleValue))] public int RecycleValue = 0;

    [Separator]

    [SerializeReference][SR] public List<Executable> Commands;







}
