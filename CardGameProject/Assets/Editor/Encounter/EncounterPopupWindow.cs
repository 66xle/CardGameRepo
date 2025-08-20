using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class EncounterPopupWindow : PopupWindow
{
    private EncounterData newEncounter;

    void CreateGUI()
    {
        if (addButtonPressed)
            AddButton();
        else if (renameButtonPressed)
            RenameButton();
    }

    void AddButton()
    {
        var label = new Label("CREATE ENCOUNTER");
        rootVisualElement.Add(label);

        Info();

        var createButton = new Button();
        createButton.style.marginTop = 20;
        createButton.style.marginBottom = 10;
        createButton.style.height = 50;
        createButton.style.fontSize = 20;
        createButton.text = "Create Encounter";
        createButton.clicked += () => CreateEncounter(newEncounter.EncounterName);
        rootVisualElement.Add(createButton);

        var cancelButton = new Button();
        cancelButton.style.height = 40;
        cancelButton.style.fontSize = 15;
        cancelButton.text = "Cancel";
        cancelButton.clicked += () => CloseWindow();
        rootVisualElement.Add(cancelButton);
    }

    void RenameButton()
    {
        var label = new Label("RENAME ENCOUNTER");
        label.style.marginBottom = 10;
        rootVisualElement.Add(label);

        // Create textfield
        var textField = new TextField();
        EncounterData selected = window.list.selectedItem as EncounterData;
        string path = AssetDatabase.GUIDToAssetPath(selected.Guid);
        string fileName = Path.GetFileNameWithoutExtension(path);
        textField.value = fileName;
        rootVisualElement.Add(textField);

        var createButton = new UnityEngine.UIElements.Button();
        createButton.style.marginTop = 20;
        createButton.style.marginBottom = 10;
        createButton.style.height = 50;
        createButton.style.fontSize = 20;
        createButton.text = "Rename Encounter";
        createButton.clicked += () => RenameEncounter(selected, fileName, textField.value);
        rootVisualElement.Add(createButton);

        var cancelButton = new UnityEngine.UIElements.Button();
        cancelButton.style.height = 40;
        cancelButton.style.fontSize = 15;
        cancelButton.text = "Cancel";
        cancelButton.clicked += () => CloseWindow();
        rootVisualElement.Add(cancelButton);
    }

    void Info()
    {
        newEncounter = new EncounterData();

        SerializedObject serializeObj = new SerializedObject(newEncounter);
        SerializedProperty property = serializeObj.GetIterator();
        property.Next(true);

        while (property.NextVisible(false))
        {
            PropertyField prop = new PropertyField(property);

            prop.SetEnabled(property.name != "m-Script");
            prop.Bind(serializeObj);

            if (property.name == "EncounterName")
            {
                rootVisualElement.Add(prop);
                return;
            }
        }
    }

    void CreateEncounter(string fileName)
    {
        if (string.IsNullOrEmpty(fileName) || string.IsNullOrWhiteSpace(fileName))
        {
            EditorUtility.DisplayDialog($"Error", $"Name empty", "Ok");
            return;
        }


        EncounterData loadedAsset = AssetDatabase.LoadAssetAtPath($"Assets/ScriptableObjects/Encounter/{fileName}.asset", typeof(EncounterData)) as EncounterData;

        if (loadedAsset != null)
        {
            EditorUtility.DisplayDialog($"Error", $"Encounter already exists", "Ok");
            return;
        }
        else
        {
            EncounterData encounter = CreateNewEncounter(newEncounter);

            AssetDatabase.CreateAsset(encounter, $"Assets/ScriptableObjects/Encounter/{fileName}.asset");

            window.CreateListView();
        }

        CloseWindow();
    }

    void RenameEncounter(EncounterData selectedData, string oldFileName, string newFileName)
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

        EncounterData loadedAsset = AssetDatabase.LoadAssetAtPath($"Assets/ScriptableObjects/Encounter/{newFileName}.asset", typeof(EncounterData)) as EncounterData;

        if (loadedAsset != null)
        {
            EditorUtility.DisplayDialog($"Error", $"Encounter already exists", "Ok");
            return;
        }

        // Clear selection
        window.list.ClearSelection();
        window.rootVisualElement.Query<Box>("encounter-info").First().Clear();
        window.list.itemsSource = null;


        EncounterData encounter = CreateNewEncounter(selectedData);

        // Delete then create asset
        AssetDatabase.DeleteAsset($"Assets/ScriptableObjects/Encounter/{oldFileName}.asset");
        AssetDatabase.CreateAsset(encounter, $"Assets/ScriptableObjects/Encounter/{newFileName}.asset");

        window.CreateListView();
        CloseWindow();
    }

    EncounterData CreateNewEncounter(EncounterData newData)
    {
        EncounterData data = new EncounterData(newData);

        return data;
    }

    private void CloseWindow()
    {
        window.isPopupActive = false;
        Close();
    }

}
