using System.Collections;
using System.Collections.Generic;
using SerializeReferenceEditor;
using UnityEngine;
using MyBox;
using System.Reflection;
using UnityEngine.Analytics;
using UnityEditor;



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

    private void OnValidate()
    {
        ValuesToReference.Clear();

        CheckCommandsForValues(Commands);
    } 

    private void CheckCommandsForValues(List<Executable> commands)
    {
        foreach (Executable command in commands)
        {
            if (command == null) continue;

            if (command is Condition)
            {
                if (command.IsUsingValue)
                    AddValueToReferenceList(command);

                Condition condition = command as Condition;
                CheckCommandsForValues(condition.Commands);
                continue;
            }

            if (!command.IsUsingValue) continue;


            AddValueToReferenceList(command);
        }
    }

    public void AddValueToReferenceList(Executable command)
    {
        if (command is BlockCommand)
        {
            ValuesToReference.Add(new Vector2(1, command.Value));

            return;
        }
        else if (command is HealCommand)
        {
            ValuesToReference.Add(new Vector2(2, command.Value));

            return;
        }

        ValuesToReference.Add(new Vector2(0, command.Value));
    }

}
