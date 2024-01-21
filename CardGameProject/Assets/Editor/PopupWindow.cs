using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UIElements;

public class PopupWindow : EditorWindow
{

    public CardEditorWindow window;

    public CardEditorWindow Window { get { return window; } set { window = value; } }


    private void OnLostFocus()
    {
        Close();
    }

    void CreateGUI()
    {
        var label = new Label("Name Card");
        rootVisualElement.Add(label);

        TextField field = new TextField();
        rootVisualElement.Add(field);

        var createButton = new Button();
        createButton.text = "Create Card";
        createButton.clicked += () => CreateCard(field.text);
        rootVisualElement.Add(createButton);

        var cancelButton = new Button();
        cancelButton.text = "Cancel";
        cancelButton.clicked += () => Close();
        rootVisualElement.Add(cancelButton); 
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
            card.name = fileName;
            AssetDatabase.CreateAsset(card, $"Assets/ScriptableObjects/Cards/{fileName}.asset");

            window.CreateCardListView();
        }

        Close();
    }
}
