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

    public string Category { get { return categoryField.value; } }
    public string NextEvent { get { return eventField.value; } }

    public new class UxmlFactory : UxmlFactory<SettingView, UxmlTraits> { }

    public SettingView()
    {
      
    }

    public void ClearSetting()
    {
        Clear();
    }

    public void DrawElements(List<Event> events, Event selectedEvent)
    {
        ClearSetting();

        var categoryChoices = new List<string> { "Random", "Cycle", "Main" };

        // Create a new field and assign it its value.
        categoryField = CreatePopupField("Category", categoryChoices, callback =>
        {
            categoryField.value = callback.newValue;
        });
        categoryField.value = selectedEvent.category;
        Add(categoryField);


        var eventList = new List<string> { "None"};
        events.ForEach(e => eventList.Add(Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(e.guid))));
        eventList.Remove(Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(selectedEvent.guid)));

        // Create a new field and assign it its value.
        eventField = CreatePopupField("Next Event", eventList, callback =>
        {
            eventField.value = callback.newValue;
         });
        eventField.value = selectedEvent.nextEvent;
        Add(eventField);

    }

    PopupField<string> CreatePopupField(string name, List<string> choices, EventCallback<ChangeEvent<string>> onValueChanged = null)
    {
        PopupField<string> popupField = new PopupField<string>(name, choices, 0);

        if (onValueChanged != null)
        {
            popupField.RegisterValueChangedCallback(onValueChanged);
        }

        return popupField;
    }
}
