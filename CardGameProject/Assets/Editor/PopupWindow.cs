using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.UI;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PopupWindow : EditorWindow
{
    public CardEditorWindow window;
    public DialogueEditor dialogueWindow;

    private Card newCard;

    public bool addCardButtonPressed = false;
    public bool renameCardButtonPressed = false;
    public bool addEventButtonPressed = false;
    public bool renameEventButtonPressed = false;

    public CardEditorWindow Window { get { return window; } set { window = value; } }


    void CreateGUI()
    {
        if (addCardButtonPressed)
            AddCard();
        else if (renameCardButtonPressed)
            RenameCard();
        else if (addEventButtonPressed)
            AddEvent();
        else if (renameEventButtonPressed)
            RenameEvent();
    }

    void AddEvent()
    {
        var label = new Label("CREATE EVENT");
        rootVisualElement.Add(label);

        // Create textfield
        var textField = new TextField();
        textField.value = " ";
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

        // Delete then create asset
        AssetDatabase.DeleteAsset($"Assets/ScriptableObjects/Events/{oldFileName}.asset");
        AssetDatabase.CreateAsset(newEvent, $"Assets/ScriptableObjects/Events/{newFileName}.asset");

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

    #region Cards

    void AddCard()
    {
        var label = new Label("CREATE CARD");
        rootVisualElement.Add(label);

        CardInfo();

        var createButton = new UnityEngine.UIElements.Button();
        createButton.text = "Create Card";
        createButton.clicked += () => CreateCard(newCard.name);
        rootVisualElement.Add(createButton);

        var cancelButton = new UnityEngine.UIElements.Button();
        cancelButton.text = "Cancel";
        cancelButton.clicked += () => CloseWindow();
        rootVisualElement.Add(cancelButton);
    }

    void RenameCard()
    {
        var label = new Label("RENAME CARD");
        rootVisualElement.Add(label);

        // Create textfield
        var textField = new TextField();
        Card selectedCard = window.cardList.selectedItem as Card;
        string path = AssetDatabase.GUIDToAssetPath(selectedCard.guid);
        string fileName = Path.GetFileNameWithoutExtension(path);
        textField.value = fileName;
        rootVisualElement.Add(textField);

        var createButton = new UnityEngine.UIElements.Button();
        createButton.text = "Rename Card";
        createButton.clicked += () => RenameCard(selectedCard, fileName, textField.value);
        rootVisualElement.Add(createButton);

        var cancelButton = new UnityEngine.UIElements.Button();
        cancelButton.text = "Cancel";
        cancelButton.clicked += () => CloseWindow();
        rootVisualElement.Add(cancelButton);
    }

    void CardInfo()
    {
        newCard = new Card();

        SerializedObject serializeCard = new SerializedObject(newCard);
        SerializedProperty cardProperty = serializeCard.GetIterator();
        cardProperty.Next(true);

        while (cardProperty.NextVisible(false))
        {
            PropertyField prop = new PropertyField(cardProperty);

            prop.SetEnabled(cardProperty.name != "m-Script");
            prop.Bind(serializeCard);
            rootVisualElement.Add(prop);
        }
    }

    void CreateCard(string fileName)
    {
        if (string.IsNullOrEmpty(fileName) || string.IsNullOrWhiteSpace(fileName))
        {
            EditorUtility.DisplayDialog($"Error", $"Name empty", "Ok");
            return;
        }

        Card loadedAsset = AssetDatabase.LoadAssetAtPath($"Assets/ScriptableObjects/Cards/{fileName}.asset", typeof(Card)) as Card;

        if (loadedAsset != null)
        {
            // If Card exists
            EditorUtility.DisplayDialog($"Error", $"Card already exists", "Ok");
            return;
        }
        else
        {
            Card card = CreateNewCard(newCard);

            AssetDatabase.CreateAsset(card, $"Assets/ScriptableObjects/Cards/{fileName}.asset");

            window.CreateCardListView();
        }

        CloseWindow();
    }

    void RenameCard(Card selectedCard, string oldFileName, string newFileName)
    {
        // Name is the same
        if (newFileName.Equals(Path.GetFileNameWithoutExtension(oldFileName)))
        {
            CloseWindow();
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
        window.cardList.ClearSelection();
        window.rootVisualElement.Query<Box>("card-info").First().Clear();
        window.cardList.itemsSource = null;


        Card card = CreateNewCard(selectedCard);

        // Delete then create asset
        AssetDatabase.DeleteAsset($"Assets/ScriptableObjects/Cards/{oldFileName}.asset");
        AssetDatabase.CreateAsset(card, $"Assets/ScriptableObjects/Cards/{newFileName}.asset");

        window.CreateCardListView();
        CloseWindow();
    }

    Card CreateNewCard(Card newCard)
    {
        Card card = new Card();
        card.name = newCard.name;
        card.description = newCard.description;
        card.flavour = newCard.flavour;
        card.cardType = newCard.cardType;
        card.value = newCard.value;
        card.cost = newCard.cost;
        card.recycleValue = newCard.recycleValue;
        card.image = newCard.image;
        card.frame = newCard.frame;

        return card;
    }

    #endregion

    private void CloseWindow()
    {
        window.isPopupActive = false;
        addCardButtonPressed = false;
        renameCardButtonPressed = false;
        Close();
    }

    private void CloseDialogueWindow()
    {
        dialogueWindow.isPopupActive = false;
        addEventButtonPressed = false;
        renameEventButtonPressed = false;
        Close();
    }
}
