using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelPopupWindow : PopupWindow
{
    private LevelData newLevel;

    void CreateGUI()
    {
        if (addButtonPressed)
            AddLevel();
        else if (renameButtonPressed)
            RenameLevel();
    }

    void AddLevel()
    {
        var label = new Label("CREATE LEVEL");
        rootVisualElement.Add(label);

        LevelInfo();

        var createButton = new Button();
        createButton.text = "Create Level";
        createButton.clicked += () => CreateLevel(newLevel.LevelName);
        rootVisualElement.Add(createButton);

        var cancelButton = new Button();
        cancelButton.text = "Cancel";
        cancelButton.clicked += () => CloseWindow();
        rootVisualElement.Add(cancelButton);
    }

    void RenameLevel()
    {
        var label = new Label("RENAME LEVEL");
        rootVisualElement.Add(label);

        // Create textfield
        var textField = new TextField();
        LevelData selectedLevelData = window.list.selectedItem as LevelData;
        string path = AssetDatabase.GUIDToAssetPath(selectedLevelData.Guid);
        string fileName = Path.GetFileNameWithoutExtension(path);
        textField.value = fileName;
        rootVisualElement.Add(textField);

        var createButton = new UnityEngine.UIElements.Button();
        createButton.text = "Rename Level";
        createButton.clicked += () => RenameLevel(selectedLevelData, fileName, textField.value);
        rootVisualElement.Add(createButton);

        var cancelButton = new UnityEngine.UIElements.Button();
        cancelButton.text = "Cancel";
        cancelButton.clicked += () => CloseWindow();
        rootVisualElement.Add(cancelButton);
    }

    void LevelInfo()
    {
        newLevel = new LevelData();

        SerializedObject serializeObj = new SerializedObject(newLevel);
        SerializedProperty property = serializeObj.GetIterator();
        property.Next(true);

        while (property.NextVisible(false))
        {
            PropertyField prop = new PropertyField(property);

            prop.SetEnabled(property.name != "m-Script");
            prop.Bind(serializeObj);
            rootVisualElement.Add(prop);
        }
    }

    void CreateLevel(string fileName)
    {
        if (string.IsNullOrEmpty(fileName) || string.IsNullOrWhiteSpace(fileName))
        {
            EditorUtility.DisplayDialog($"Error", $"Name empty", "Ok");
            return;
        }


        LevelData loadedAsset = AssetDatabase.LoadAssetAtPath($"Assets/ScriptableObjects/Level/{fileName}.asset", typeof(LevelData)) as LevelData;

        if (loadedAsset != null)
        {
            // If Level exists
            EditorUtility.DisplayDialog($"Error", $"Level already exists", "Ok");
            return;
        }
        else
        {
            LevelData level = CreateNewLevel(newLevel);

            AssetDatabase.CreateAsset(level, $"Assets/ScriptableObjects/Level/{fileName}.asset");

            window.CreateListView();
        }

        CloseWindow();
    }

    void RenameLevel(LevelData selectedLevelData, string oldFileName, string newFileName)
    {
        // Name is the same
        if (newFileName.Equals(Path.GetFileNameWithoutExtension(oldFileName)))
        {
            EditorUtility.DisplayDialog($"Error", $"Name is the same", "Ok");
            return;
        }

        // Name is Blank
        if (string.IsNullOrEmpty(newFileName) || string.IsNullOrWhiteSpace(newFileName))
        {
            EditorUtility.DisplayDialog($"Error", $"Name empty", "Ok");
            return;
        }

        LevelData loadedAsset = AssetDatabase.LoadAssetAtPath($"Assets/ScriptableObjects/Level/{newFileName}.asset", typeof(LevelData)) as LevelData;

        if (loadedAsset != null)
        {
            // If Level exists
            EditorUtility.DisplayDialog($"Error", $"Level already exists", "Ok");
            return;
        }

        // Clear selection
        window.list.ClearSelection();
        window.rootVisualElement.Query<Box>("level-info").First().Clear();
        window.list.itemsSource = null;


        LevelData level = CreateNewLevel(selectedLevelData);

        // Delete then create asset
        AssetDatabase.DeleteAsset($"Assets/ScriptableObjects/Level/{oldFileName}.asset");
        AssetDatabase.CreateAsset(level, $"Assets/ScriptableObjects/Level/{newFileName}.asset");

        window.CreateListView();
        CloseWindow();
    }

    LevelData CreateNewLevel(LevelData newLevelData)
    {
        LevelData level = new LevelData();
        level.name = newLevelData.LevelName;
        level.Prefab = newLevelData.Prefab;

        return level;
    }

    private void CloseWindow()
    {
        window.isPopupActive = false;
        Close();
    }

}
