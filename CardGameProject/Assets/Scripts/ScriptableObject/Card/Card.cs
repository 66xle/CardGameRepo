using System.Collections;
using System.Collections.Generic;
using SerializeReferenceEditor;
using UnityEngine;
using MyBox;
using System.Reflection;
using UnityEngine.Analytics;



[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject
{
    [ReadOnly] public string guid;
    [ReadOnly] public string InGameGUID;

    public string cardName;
    [TextArea] public string description;
    [HideInInspector] public string displayDescription;
    [TextArea] public string flavour;

    [Header("Card Image")]
    public Sprite image;
    public Sprite frame;

    [Header("Card Info")]
    public int cost;
    public int recycleValue;

    [Separator]

    public List<float> valuesToReference = new();

    [Separator]

    [SerializeReference][SR] public List<Executable> commands = new List<Executable>();


    private void OnValidate()
    {
        valuesToReference.Clear();

        CheckCommandsForValues(commands);

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
                    valuesToReference.Add(command.Value);

                Condition condition = command as Condition;
                CheckCommandsForValues(condition.Commands);
                continue;
            }

            if (!command.IsUsingValue) continue;

            valuesToReference.Add(command.Value);
        }
    }

    public void GenerateDisplayDescription()
    {
        displayDescription = description;

        for (int i = 0; i < valuesToReference.Count; i++)
        {
            float value = valuesToReference[i];

            displayDescription = displayDescription.Replace($"#{i}", value.ToString());
        }
    }
}
