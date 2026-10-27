using System;
using PixelCrushers.DialogueSystem;
using StoryFlow.Data;
using UnityEngine;

public static class StoryFlowHelper
{
    public static string GetData(StoryFlowNode node, string key)
    {
        if (node == null || node.Data == null) return null;
        string value;
        return node.Data.TryGetValue(key, out value) ? value : null;
    }

    public static bool NodeHasTag(StoryFlowNode node, string tag)
    {
        string tags = GetData(node, "tags");
        return !string.IsNullOrEmpty(tags) && tags.IndexOf(tag, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    public static string GetVariableValue(StoryFlowVariable variable, out FieldType fieldType)
    {
        switch (variable.Type)
        {
            case StoryFlowVariableType.Boolean:
                fieldType = FieldType.Boolean;
                return variable.Value.BoolValue ? "true" : "false";
            case StoryFlowVariableType.Integer:
                fieldType = FieldType.Number;
                return variable.Value.IntValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
            case StoryFlowVariableType.Float:
                fieldType = FieldType.Number;
                return variable.Value.FloatValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
            default:
                fieldType = FieldType.Text;
                return variable.Value.StringValue ?? string.Empty;
        }
    }

    


}
