using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class WeaponPopupWindow : PopupWindow
{
    private WeaponData newWeapon;

    void CreateGUI()
    {
        if (addButtonPressed)
            AddWeapon();
        else if (renameButtonPressed)
            RenameWeapon();
    }

    void AddWeapon()
    {
        var label = new Label("CREATE WEAPON");
        rootVisualElement.Add(label);

        WeaponInfo();

        var createButton = new Button();
        createButton.text = "Create Weapon";
        createButton.clicked += () => CreateWeapon(newWeapon.weaponName);
        rootVisualElement.Add(createButton);

        var cancelButton = new Button();
        cancelButton.text = "Cancel";
        cancelButton.clicked += () => CloseWindow();
        rootVisualElement.Add(cancelButton);
    }

    void RenameWeapon()
    {
        var label = new Label("RENAME WEAPON");
        rootVisualElement.Add(label);

        // Create textfield
        var textField = new TextField();
        WeaponData selectedWeaponData = window.list.selectedItem as WeaponData;
        string path = AssetDatabase.GUIDToAssetPath(selectedWeaponData.guid);
        string fileName = Path.GetFileNameWithoutExtension(path);
        textField.value = fileName;
        rootVisualElement.Add(textField);

        var createButton = new UnityEngine.UIElements.Button();
        createButton.text = "Rename Weapon";
        createButton.clicked += () => RenameWeapon(selectedWeaponData, fileName, textField.value);
        rootVisualElement.Add(createButton);

        var cancelButton = new UnityEngine.UIElements.Button();
        cancelButton.text = "Cancel";
        cancelButton.clicked += () => CloseWindow();
        rootVisualElement.Add(cancelButton);
    }

    void WeaponInfo()
    {
        newWeapon = new WeaponData();

        SerializedObject serializeCard = new SerializedObject(newWeapon);
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

    void CreateWeapon(string fileName)
    {
        if (string.IsNullOrEmpty(fileName) || string.IsNullOrWhiteSpace(fileName))
        {
            EditorUtility.DisplayDialog($"Error", $"Name empty", "Ok");
            return;
        }

        WeaponData loadedAsset = AssetDatabase.LoadAssetAtPath($"Assets/ScriptableObjects/Weapon/{fileName}.asset", typeof(WeaponData)) as WeaponData;

        if (loadedAsset != null)
        {
            // If Weapon exists
            EditorUtility.DisplayDialog($"Error", $"Weapon already exists", "Ok");
            return;
        }
        else
        {
            WeaponData card = CreateNewWeapon(newWeapon);

            AssetDatabase.CreateAsset(card, $"Assets/ScriptableObjects/Cards/{fileName}.asset");

            window.CreateListView();
        }

        CloseWindow();
    }

    void RenameWeapon(WeaponData selectedWeaponData, string oldFileName, string newFileName)
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
        window.rootVisualElement.Query<Box>("weapon-info").First().Clear();
        window.list.itemsSource = null;


        WeaponData weapon = CreateNewWeapon(selectedWeaponData);

        // Delete then create asset
        AssetDatabase.DeleteAsset($"Assets/ScriptableObjects/Weapon/{oldFileName}.asset");
        AssetDatabase.CreateAsset(weapon, $"Assets/ScriptableObjects/Weapon/{newFileName}.asset");

        window.CreateListView();
        CloseWindow();
    }

    WeaponData CreateNewWeapon(WeaponData newWeaponData)
    {
        WeaponData weapon = new WeaponData();
        weapon.name = newWeaponData.name;
        weapon.description = newWeaponData.description;
        weapon.prefab = newWeaponData.prefab;
        weapon.cards = newWeaponData.cards;

        return weapon;
    }

    private void CloseWindow()
    {
        window.isPopupActive = false;
        Close();
    }

}