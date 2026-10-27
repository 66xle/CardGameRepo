using PixelCrushers.DialogueSystem;
using TextMateSharp.Grammars;
using UnityEngine;

public static class LuaHelper
{
    public static void AppendCondition(DialogueEntry entry, string condition)
    {
        if (entry == null || string.IsNullOrEmpty(condition)) return;
        if (!string.IsNullOrEmpty(entry.conditionsString)) entry.conditionsString += " and ";
        entry.conditionsString += condition;
    }

    public static void AppendScript(DialogueEntry entry, string script)
    {
        if (entry == null || string.IsNullOrEmpty(script)) return;
        entry.userScript = (entry.userScript ?? string.Empty) + script;
    }

    public static void AppendSequence(DialogueEntry entry, string sequence)
    {
        if (entry == null || string.IsNullOrEmpty(sequence)) return;
        entry.Sequence = (entry.Sequence ?? string.Empty) + sequence;
    }

    public static string LuaEquals(string variableName, string value)
    {
        return $"(Variable[\"{variableName}\"] == {value})";
    }

    public static string LuaSet(string variableName, string value)
    {
        return $"Variable[\"{variableName}\"] = {value};";
    }
}
