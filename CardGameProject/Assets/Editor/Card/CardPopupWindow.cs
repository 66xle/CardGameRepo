using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;

public class CardPopupWindow : PopupWindow
{
    private Card newCard;

    void CreateGUI()
    {
        if (addButtonPressed)
            AddCard();
        else if (renameButtonPressed)
            RenameCard();
    }


    void AddCard()
    {
        var label = new Label("CREATE CARD");
        rootVisualElement.Add(label);

        CardInfo();

        var createButton = new UnityEngine.UIElements.Button();
        createButton.text = "Create Card";
        createButton.clicked += () => CreateCard(newCard.CardName);
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
        Card selectedCard = window.list.selectedItem as Card;
        string path = AssetDatabase.GUIDToAssetPath(selectedCard.Guid);
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

        Card card = CreateNewCard(newCard);
        AssetDatabase.CreateAsset(card, $"Assets/ScriptableObjects/Cards/{fileName}.asset");

        
        window.CreateListView();
        window.list.selectedIndex = 0;
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
        window.list.ClearSelection();
        window.rootVisualElement.Query<Box>("card-info").First().Clear();
        window.list.itemsSource = null;


        Card card = CreateNewCard(selectedCard);

        // Delete then create asset
        AssetDatabase.DeleteAsset($"Assets/ScriptableObjects/Cards/{oldFileName}.asset");
        AssetDatabase.CreateAsset(card, $"Assets/ScriptableObjects/Cards/{newFileName}.asset");

        window.CreateListView();
        CloseWindow();
    }

    Card CreateNewCard(Card newCard)
    {
        Card card = new Card();
        card.CardName = newCard.CardName;
        card.Description = newCard.Description;
        card.Flavour = newCard.Flavour;
        card.ValuesToReference = newCard.ValuesToReference;
        card.Cost = newCard.Cost;
        card.RecycleValue = newCard.RecycleValue;
        card.Image = newCard.Image;
        card.Frame = newCard.Frame;

        return card;
    }


    private void CloseWindow()
    {
        window.isPopupActive = false;
        Close();
    }
}
