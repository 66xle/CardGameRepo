using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;

public class DialoguePopupWindow : PopupWindow
{
    public DialogueEditor dialogueWindow;

    void CreateGUI()
    {
        if (addButtonPressed)
            AddEvent();
        else if (renameButtonPressed)
            RenameEvent();
    }


    void AddEvent()
    {
        var label = new Label("CREATE EVENT");
        rootVisualElement.Add(label);

        // Create textfield
        var textField = new TextField();
        textField.value = "";
        rootVisualElement.Add(textField);

        var choices = new List<string> { "Single Event", "Linked Event" };

        // Create a new field and assign it its value.
        var popupField = new PopupField<string>("Event Type", choices, 0);
        popupField.value = choices[0];
        rootVisualElement.Add(popupField);


        var createButton = new UnityEngine.UIElements.Button();
        createButton.text = "Create Event";
        createButton.clicked += () => CreateEvent(textField.value, popupField);
        rootVisualElement.Add(createButton);

        var cancelButton = new UnityEngine.UIElements.Button();
        cancelButton.text = "Cancel";
        cancelButton.clicked += () => CloseDialogueWindow();
        rootVisualElement.Add(cancelButton);
    }

    void RenameEvent()
    {
        var label = new Label("RENAME EVENT");
        rootVisualElement.Add(label);

        // Create textfield
        var textField = new TextField();
        Event selectedEvent = dialogueWindow.eventList.selectedItem as Event;
        string path = AssetDatabase.GUIDToAssetPath(selectedEvent.guid);
        string fileName = Path.GetFileNameWithoutExtension(path);
        textField.value = fileName;
        rootVisualElement.Add(textField);

        var createButton = new UnityEngine.UIElements.Button();
        createButton.text = "Rename Event";
        createButton.clicked += () => RenameEvent(selectedEvent, fileName, textField.value);
        rootVisualElement.Add(createButton);

        var cancelButton = new UnityEngine.UIElements.Button();
        cancelButton.text = "Cancel";
        cancelButton.clicked += () => CloseDialogueWindow();
        rootVisualElement.Add(cancelButton);
    }

    void CreateEvent(string fileName, PopupField<string> popupField)
    {
        if (string.IsNullOrEmpty(fileName) || string.IsNullOrWhiteSpace(fileName))
        {
            EditorUtility.DisplayDialog($"Error", $"Name empty", "Ok");
            return;
        }

        Event loadedAsset = AssetDatabase.LoadAssetAtPath($"Assets/ScriptableObjects/Events/{fileName}.asset", typeof(Event)) as Event;

        if (loadedAsset != null)
        {
            // If Event exists
            EditorUtility.DisplayDialog($"Error", $"Event already exists", "Ok");
            return;
        }
        else
        {
            Event newEvent = new Event();
            newEvent.type = popupField.value;
            newEvent.category = "Random";
            newEvent.nextEvent = null;

            AssetDatabase.CreateAsset(newEvent, $"Assets/ScriptableObjects/Events/{fileName}.asset");
            dialogueWindow.eventList.Clear();
            dialogueWindow.CreateEventListView();
        }

        CloseDialogueWindow();
    }

    void RenameEvent(Event selectedEvent, string oldFileName, string newFileName)
    {
        // Name is the same
        if (newFileName.Equals(Path.GetFileNameWithoutExtension(oldFileName)))
        {
            CloseDialogueWindow();
            return;
        }

        // Name is Blank
        if (string.IsNullOrEmpty(newFileName) || string.IsNullOrWhiteSpace(newFileName))
        {
            EditorUtility.DisplayDialog($"Error", $"Name empty", "Ok");
            return;
        }

        //if (!Regex.IsMatch(newFileName, @"^[a-zA-Z]+$"))
        //{
        //    EditorUtility.DisplayDialog($"Error", $"Must only have letters", "Ok");
        //    return;
        //}

        // Clear selection
        dialogueWindow.eventList.ClearSelection();
        dialogueWindow.ClearGraph();
        dialogueWindow.eventList.itemsSource = null;


        Event newEvent = CreateNewEvent(selectedEvent);
        newEvent.category = selectedEvent.category;
        newEvent.nextEvent = selectedEvent.nextEvent;

        // Change next event that is linked to the renamed event
        dialogueWindow.FindAllEvents(out List<Event> events);


        List<Event> eventsToBeRenamed = new List<Event>();
        foreach (Event evt in events)
        {
            if (evt.nextEvent == null)
            {
                continue;
            }

            if (evt.nextEvent.name == oldFileName)
                eventsToBeRenamed.Add(evt);
        }

        // Delete then create asset
        AssetDatabase.DeleteAsset($"Assets/ScriptableObjects/Events/{oldFileName}.asset");
        AssetDatabase.CreateAsset(newEvent, $"Assets/ScriptableObjects/Events/{newFileName}.asset");

        Event renamedEvent = AssetDatabase.LoadAssetAtPath<Event>($"Assets/ScriptableObjects/Events/{newFileName}.asset");
        eventsToBeRenamed.ForEach(evt => evt.nextEvent = renamedEvent);


        dialogueWindow.eventList.Clear();
        dialogueWindow.CreateEventListView();
        CloseDialogueWindow();
    }

    Event CreateNewEvent(Event oldEvent)
    {
        Event newEvent = new Event();

        newEvent.name = oldEvent.name;
        newEvent.DialogueNodeData = oldEvent.DialogueNodeData;
        newEvent.listChildData = oldEvent.listChildData;
        newEvent.type = oldEvent.type;

        return newEvent;
    }

    private void CloseDialogueWindow()
    {
        dialogueWindow.isPopupActive = false;
        Close();
    }
}
