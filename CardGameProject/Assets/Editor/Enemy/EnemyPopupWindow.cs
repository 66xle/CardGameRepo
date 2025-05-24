using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyPopupWindow : PopupWindow
{
    private EnemyData newEnemy;

    void CreateGUI()
    {
        if (addButtonPressed)
            AddEnemy();
        else if (renameButtonPressed)
            RenameEnemy();
    }

    void AddEnemy()
    {
        var label = new Label("CREATE ENEMY");
        rootVisualElement.Add(label);

        EnemyInfo();

        var createButton = new Button();
        createButton.text = "Create Enemy";
        createButton.clicked += () => CreateEnemy(newEnemy.Name);
        rootVisualElement.Add(createButton);

        var cancelButton = new Button();
        cancelButton.text = "Cancel";
        cancelButton.clicked += () => CloseWindow();
        rootVisualElement.Add(cancelButton);
    }

    void RenameEnemy()
    {
        var label = new Label("RENAME ENEMY");
        rootVisualElement.Add(label);

        // Create textfield
        var textField = new TextField();
        EnemyData selectedEnemyData = window.list.selectedItem as EnemyData;
        string path = AssetDatabase.GUIDToAssetPath(selectedEnemyData.Guid);
        string fileName = Path.GetFileNameWithoutExtension(path);
        textField.value = fileName;
        rootVisualElement.Add(textField);

        var createButton = new UnityEngine.UIElements.Button();
        createButton.text = "Rename Enemy";
        createButton.clicked += () => RenameEnemy(selectedEnemyData, fileName, textField.value);
        rootVisualElement.Add(createButton);

        var cancelButton = new UnityEngine.UIElements.Button();
        cancelButton.text = "Cancel";
        cancelButton.clicked += () => CloseWindow();
        rootVisualElement.Add(cancelButton);
    }

    void EnemyInfo()
    {
        newEnemy = new EnemyData();

        SerializedObject serializeEnemy = new SerializedObject(newEnemy);
        SerializedProperty enemyProperty = serializeEnemy.GetIterator();
        enemyProperty.Next(true);

        while (enemyProperty.NextVisible(false))
        {
            PropertyField prop = new PropertyField(enemyProperty);

            prop.SetEnabled(enemyProperty.name != "m-Script");
            prop.Bind(serializeEnemy);
            rootVisualElement.Add(prop);
        }
    }

    void CreateEnemy(string fileName)
    {
        if (string.IsNullOrEmpty(fileName) || string.IsNullOrWhiteSpace(fileName))
        {
            EditorUtility.DisplayDialog($"Error", $"Name empty", "Ok");
            return;
        }


        EnemyData loadedAsset = AssetDatabase.LoadAssetAtPath($"Assets/ScriptableObjects/Enemy/{fileName}.asset", typeof(EnemyData)) as EnemyData;

        if (loadedAsset != null)
        {
            // If Enemy exists
            EditorUtility.DisplayDialog($"Error", $"Enemy already exists", "Ok");
            return;
        }
        else
        {
            EnemyData enemy = CreateNewEnemy(newEnemy);

            AssetDatabase.CreateAsset(enemy, $"Assets/ScriptableObjects/Enemy/{fileName}.asset");

            window.CreateListView();
        }

        CloseWindow();
    }

    void RenameEnemy(EnemyData selectedEnemyData, string oldFileName, string newFileName)
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

        EnemyData loadedAsset = AssetDatabase.LoadAssetAtPath($"Assets/ScriptableObjects/Enemy/{newFileName}.asset", typeof(EnemyData)) as EnemyData;

        if (loadedAsset != null)
        {
            // If Enemy exists
            EditorUtility.DisplayDialog($"Error", $"Enemy already exists", "Ok");
            return;
        }

        // Clear selection
        window.list.ClearSelection();
        window.rootVisualElement.Query<Box>("enemy-info").First().Clear();
        window.list.itemsSource = null;


        EnemyData enemy = CreateNewEnemy(selectedEnemyData);

        // Delete then create asset
        AssetDatabase.DeleteAsset($"Assets/ScriptableObjects/Enemy/{oldFileName}.asset");
        AssetDatabase.CreateAsset(enemy, $"Assets/ScriptableObjects/Enemy/{newFileName}.asset");

        window.CreateListView();
        CloseWindow();
    }

    EnemyData CreateNewEnemy(EnemyData newEnemyData)
    {
        EnemyData enemy = new EnemyData();
        enemy.Name = newEnemyData.Name;
        enemy.Health = newEnemyData.Health;
        enemy.Guard = newEnemyData.Guard;
        enemy.Prefab = newEnemyData.Prefab;
        enemy.Guid = newEnemyData.Guid;
        enemy.WeaponType = newEnemyData.WeaponType;
        enemy.WeaponTypeAnimationSet = newEnemyData.WeaponTypeAnimationSet;
        enemy.Cards = newEnemyData.Cards;
        enemy.EnemyType = newEnemyData.EnemyType;


        return enemy;
    }

    private void CloseWindow()
    {
        window.isPopupActive = false;
        Close();
    }

}
