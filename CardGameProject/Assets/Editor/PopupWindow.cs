using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.UI;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class PopupWindow : EditorWindow
{
    public CardEditorWindow window;
    public DialogueEditor dialogueWindow;

    private Card newCard;
    private Event newEvent;

    public bool addCardButtonPressed = false;
    public bool renameCardButtonPressed = false;
    public bool addEventButtonPressed = false;

    public CardEditorWindow Window { get { return window; } set { window = value; } }

    void CreateGUI()
    {
        if (addCardButtonPressed)
            AddCard();
        else if (renameCardButtonPressed)
            RenameCard();
        else if (addEventButtonPressed)
            AddEvent();
    }

    void AddEvent()
    {
        var label = new Label("CREATE EVENT");
        rootVisualElement.Add(label);

        // Create textfield
        var textField = new TextField();
        textField.value = " ";
        rootVisualElement.Add(textField);

        var createButton = new Button();
        createButton.text = "Create Event";
        createButton.clicked += () => CreateEvent(textField.value);
        rootVisualElement.Add(createButton);

        var cancelButton = new Button();
        cancelButton.text = "Cancel";
        cancelButton.clicked += () => CloseDialogueWindow();
        rootVisualElement.Add(cancelButton);
    }

    void CreateEvent(string fileName)
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

            AssetDatabase.CreateAsset(newEvent, $"Assets/ScriptableObjects/Events/{fileName}.asset");

            dialogueWindow.CreateEventListView();
        }

        CloseDialogueWindow();
    }

    #region Cards

    void AddCard()
    {
        var label = new Label("CREATE CARD");
        rootVisualElement.Add(label);

        CardInfo();

        var createButton = new Button();
        createButton.text = "Create Card";
        createButton.clicked += () => CreateCard(newCard.name);
        rootVisualElement.Add(createButton);

        var cancelButton = new Button();
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

        var createButton = new Button();
        createButton.text = "Rename Card";
        createButton.clicked += () => RenameCard(selectedCard, fileName, textField.value);
        rootVisualElement.Add(createButton);

        var cancelButton = new Button();
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
        Close();
    }
}
