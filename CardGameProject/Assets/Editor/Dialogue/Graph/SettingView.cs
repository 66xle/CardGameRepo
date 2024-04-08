using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingView : VisualElement
{
    private PopupField<string> categoryField;
    private PopupField<string> eventField;

    public string GetCategory { get { return categoryField.value; } }
    public string GetNextEvent { get { return eventField.value; } }

    public new class UxmlFactory : UxmlFactory<SettingView, UxmlTraits> { }

    public SettingView()
    {
      
    }

    public void DrawElements(List<Event> events)
    {
        Clear();

        var categoryChoices = new List<string> { "All", "Cycle", "Main Mission" };

        // Create a new field and assign it its value.
        categoryField = new PopupField<string>("Category", categoryChoices, 0);
        categoryField.value = "All";
        Add(categoryField);


        var eventList = new List<string> { "None"};
        events.ForEach(e => eventList.Add(Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(e.guid))));

        // Create a new field and assign it its value.
        eventField = new PopupField<string>("Next Event", eventList, 0);
        eventField.value = "None";
        Add(eventField);

    }
}
