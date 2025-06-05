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

    [HideInInspector] public List<SerializableKeyValuePair<string, PopupText>> PopupKeyPair;

    [Header("Card Image")]
    public Sprite Image;
    public Sprite Frame;

    [Header("Card Info")]
    public int Cost;
    public int RecycleValue;

    [Separator]

    public List<float> ValuesToReference = new();

    [Separator]

    [SerializeReference][SR] public List<Executable> Commands = new List<Executable>();


    private void OnValidate()
    {
        ValuesToReference.Clear();

        CheckCommandsForValues(Commands);

        GenerateDisplayDescription();
    } 

    private void CheckCommandsForValues(List<Executable> commands)
    {
        foreach (Executable command in commands)
        {
            if (command == null) continue;

            if (command is Condition)
            {
                if (command.IsUsingValue) 
                    ValuesToReference.Add(command.Value);

                Condition condition = command as Condition;
                CheckCommandsForValues(condition.Commands);
                continue;
            }

            if (!command.IsUsingValue) continue;

            ValuesToReference.Add(command.Value);
        }
    }

    public void GenerateDisplayDescription()
    {
        DisplayDescription = Description;

        for (int i = 0; i < ValuesToReference.Count; i++)
        {
            float value = ValuesToReference[i];

            DisplayDescription = DisplayDescription.Replace($"#{i}", value.ToString());
        }
    }
}
