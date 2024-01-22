using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.UI;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class PopupWindow : EditorWindow
{

    public CardEditorWindow window;

    private Card newCard;

    public CardEditorWindow Window { get { return window; } set { window = value; } }

    void CreateGUI()
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

            AssetDatabase.CreateAsset(card, $"Assets/ScriptableObjects/Cards/{fileName}.asset");

            window.CreateCardListView();
        }

        CloseWindow();
    }

    private void CloseWindow()
    {
        window.isPopupActive = false;
        Close();
    }
}
