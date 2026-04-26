using MyBox;
using SerializeReferenceEditor;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject
{
    [ReadOnly] public string Guid;
    [ReadOnly] public string InGameGUID;

    public string CardName;
    [TextArea] public string Description;
    [HideInInspector] public string DisplayDescription;
    [HideInInspector] public string LinkDescription;
    [TextArea] public string Flavour;

    [Header("Card Image")]
    public Sprite Image;
    public Sprite Frame;

    [Header("Card Info")]
    public int Cost;
    public int RecycleValue;

    [Separator]

    [ReadOnly] public List<Vector2> ValuesToReference = new();

    [Separator]

    [SerializeReference][SR] public List<Executable> Commands = new List<Executable>();

    [HideInInspector] public List<SerializableKeyValuePair<string, PopupText>> PopupKeyPair;

    public List<CardVariant> Variants = new();

    private void OnValidate()
    {
        ValuesToReference.Clear();
        CheckCommandsForValues(Commands, ValuesToReference);

        foreach (CardVariant variant in Variants)
        {
            variant.ValuesToReference.Clear();
            CheckCommandsForValues(variant.Commands, variant.ValuesToReference);
        }
    }

    private void CheckCommandsForValues(List<Executable> commands, List<Vector2> valuesToReference)
    {
        foreach (Executable command in commands)
        {
            if (command == null) continue;

            if (command is Condition)
            {
                if (command.IsUsingValue)
                    AddValueToReferenceList(command, valuesToReference);

                Condition condition = command as Condition;
                CheckCommandsForValues(condition.Commands, valuesToReference);
                continue;
            }

            if (!command.IsUsingValue) continue;


            AddValueToReferenceList(command, valuesToReference);
        }
    }

    public void AddValueToReferenceList(Executable command, List<Vector2> valuesToReference)
    {
        if (command is BlockCommand)
        {
            valuesToReference.Add(new Vector2(1, command.Value));

            return;
        }
        else if (command is HealCommand)
        {
            valuesToReference.Add(new Vector2(2, command.Value));

            return;
        }
        else if (command is GuardCommand || command is DrawCommand)
        {
            valuesToReference.Add(new Vector2(3, command.Value));

            return;
        }

        valuesToReference.Add(new Vector2(0, command.Value));
    }

}
