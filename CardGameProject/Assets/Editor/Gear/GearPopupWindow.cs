using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class GearPopupWindow : PopupWindow
{
    private GearData newGear;

    void CreateGUI()
    {
        if (addButtonPressed)
            AddWeapon();
        else if (renameButtonPressed)
            RenameGear();
    }

    void AddWeapon()
    {
        var label = new Label("CREATE GEAR");
        label.style.marginBottom = 10;
        rootVisualElement.Add(label);

        GearInfo();

        var createButton = new Button();
        createButton.style.marginTop = 20;
        createButton.style.marginBottom = 10;
        createButton.style.height = 50;
        createButton.style.fontSize = 20;
        createButton.text = "Create Weapon";
        createButton.clicked += () => CreateGear(newGear.GearName, "Weapon");
        rootVisualElement.Add(createButton);

        var createArmour = new Button();
        createArmour.style.marginBottom = 10;
        createArmour.style.height = 50;
        createArmour.style.fontSize = 20;
        createArmour.text = "Create Armour";
        createArmour.clicked += () => CreateGear(newGear.GearName, "Armour");
        rootVisualElement.Add(createArmour);

        var cancelButton = new Button();
        cancelButton.style.height = 40;
        cancelButton.style.fontSize = 15;
        cancelButton.text = "Cancel";
        cancelButton.clicked += () => CloseWindow();
        rootVisualElement.Add(cancelButton);
    }

    void RenameGear()
    {
        var label = new Label("RENAME GEAR");
        label.style.marginBottom = 10;
        rootVisualElement.Add(label);

        // Create textfield
        var textField = new TextField();
        var selectedData = window.list.selectedItem as GearData;
        string type = "Weapon";

        if (window.list.selectedItem is ArmourData)
            type = "Armour";

        string path = AssetDatabase.GUIDToAssetPath(selectedData.Guid);
        string fileName = Path.GetFileNameWithoutExtension(path);
        textField.value = fileName;
        rootVisualElement.Add(textField);

        var createButton = new UnityEngine.UIElements.Button();
        createButton.style.marginTop = 20;
        createButton.style.marginBottom = 10;
        createButton.style.height = 50;
        createButton.style.fontSize = 20;
        createButton.text = "Rename Gear";
        createButton.clicked += () => RenameGear(selectedData, fileName, textField.value, type);
        rootVisualElement.Add(createButton);

        var cancelButton = new UnityEngine.UIElements.Button();
        cancelButton.style.height = 40;
        cancelButton.style.fontSize = 15;
        cancelButton.text = "Cancel";
        cancelButton.clicked += () => CloseWindow();
        rootVisualElement.Add(cancelButton);
    }

    void GearInfo()
    {
        newGear = new GearData();

        SerializedObject serializeGear = new SerializedObject(newGear);
        SerializedProperty property = serializeGear.GetIterator();
        property.Next(true);

        while (property.NextVisible(false))
        {
            PropertyField prop = new PropertyField(property);

            prop.SetEnabled(property.name != "m-Script");
            prop.Bind(serializeGear);

            if (property.name == "GearName")
            {
                rootVisualElement.Add(prop);
                return;
            }
        }
    }

    void CreateGear(string fileName, string type)
    {
        if (string.IsNullOrEmpty(fileName) || string.IsNullOrWhiteSpace(fileName))
        {
            EditorUtility.DisplayDialog($"Error", $"Name empty", "Ok");
            return;
        }


        GearData loadedAsset = AssetDatabase.LoadAssetAtPath($"Assets/ScriptableObjects/Gear/{fileName}.asset", typeof(GearData)) as GearData;

        if (loadedAsset != null)
        {
            // If Weapon exists
            EditorUtility.DisplayDialog($"Error", $"Gear already exists", "Ok");
            return;
        }
        else
        {
            if (type == "Weapon")
            {
                WeaponData weapon = CreateNewWeapon(newGear);

                AssetDatabase.CreateAsset(weapon, $"Assets/ScriptableObjects/Gear/{fileName}.asset");
            }
            else
            {
                ArmourData armour = CreateNewArmour(newGear);

                AssetDatabase.CreateAsset(armour, $"Assets/ScriptableObjects/Gear/{fileName}.asset");
            }

            window.CreateListView();
        }

        CloseWindow();
    }

    void RenameGear(GearData selectedData, string oldFileName, string newFileName, string type)
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

        GearData loadedAsset = AssetDatabase.LoadAssetAtPath($"Assets/ScriptableObjects/Gear/{newFileName}.asset", typeof(GearData)) as GearData;

        if (loadedAsset != null)
        {
            // If Weapon exists
            EditorUtility.DisplayDialog($"Error", $"Gear already exists", "Ok");
            return;
        }

        // Clear selection
        window.list.ClearSelection();
        window.rootVisualElement.Query<Box>("gear-info").First().Clear();
        window.list.itemsSource = null;

        if (type == "Weapon")
        {
            WeaponData weapon = RenameWeapon(selectedData as WeaponData);

            // Delete then create asset
            AssetDatabase.DeleteAsset($"Assets/ScriptableObjects/Gear/{oldFileName}.asset");
            AssetDatabase.CreateAsset(weapon, $"Assets/ScriptableObjects/Gear/{newFileName}.asset");
        }
        else
        {
            ArmourData armour = RenameArmour(selectedData as ArmourData);

            // Delete then create asset
            AssetDatabase.DeleteAsset($"Assets/ScriptableObjects/Gear/{oldFileName}.asset");
            AssetDatabase.CreateAsset(armour, $"Assets/ScriptableObjects/Gear/{newFileName}.asset");
        }

        window.CreateListView();
        CloseWindow();
    }

    ArmourData CreateNewArmour(GearData newData)
    {
        ArmourData armourData = new();
        armourData.GearName = newData.GearName;

        return armourData;
    }

    ArmourData RenameArmour(ArmourData oldData)
    {
        ArmourData armourData = new(oldData);

        return armourData;
    }

    WeaponData CreateNewWeapon(GearData newData)
    {
        WeaponData newWeaponData = new();
        newWeaponData.GearName = newData.GearName;

        return newWeaponData;
    }

    WeaponData RenameWeapon(WeaponData oldData)
    {
        WeaponData newWeaponData = new(oldData);
        newWeaponData.WeaponTypeAnimationSet = oldData.WeaponTypeAnimationSet;

        return newWeaponData;
    }

    private void CloseWindow()
    {
        

        Close();
    }

}
