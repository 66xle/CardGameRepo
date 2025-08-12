using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Constraints;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class GearEditorWindow : BaseEditorWindow
{
    GameObject gameObject;
    Editor gameObjectEditor;

    [MenuItem("Editor/Gear Editor")]
    public static void ShowWindow()
    {
        GearEditorWindow window = GetWindow<GearEditorWindow>();
        ShowWindow(window, "Gear Editor");
    }

    [InitializeOnLoadMethod]
    private static void OnLoad()
    {
        listIndex = SessionState.GetInt("gearListIndex", 0);
        isInitialized = false;
        editorReadyToInit = true;
    }

    public override void Init()
    {
        Enable("GearEditorWindow", "GearEditorStyles", "gear", "Gear");

        EditorApplication.delayCall += () => 
        {
            base.Init();

            DropdownField gearField = rootVisualElement.Query<DropdownField>("gear-filter");
            gearField.RegisterCallback<ChangeEvent<string>>((evt) =>
            {
                gearField.value = evt.newValue;
                CreateListView();
            });

            DropdownField weaponField = rootVisualElement.Query<DropdownField>("weapon-filter");
            weaponField.RegisterCallback<ChangeEvent<string>>((evt) =>
            {
                weaponField.value = evt.newValue;
                CreateListView();
            });

            DropdownField armourField = rootVisualElement.Query<DropdownField>("armour-filter");
            armourField.RegisterCallback<ChangeEvent<string>>((evt) =>
            {
                armourField.value = evt.newValue;
                CreateListView();
            });
        };
    }

    public override void CreateListView()
    {
        DropdownField dropdownField = rootVisualElement.Query<DropdownField>("gear-filter");
        DropdownField weaponFilter = rootVisualElement.Query<DropdownField>("weapon-filter");
        DropdownField armourFilter = rootVisualElement.Query<DropdownField>("armour-filter");

        List<GearData> gears = new List<GearData>();
        List<WeaponData> weapons = new List<WeaponData>();
        List<ArmourData> armours = new List<ArmourData>();

        if (dropdownField.value == "All")
        {
            weaponFilter.style.display = DisplayStyle.None;
            armourFilter.style.display = DisplayStyle.None;

            gears = FindAllGears();
        }
        else if (dropdownField.value == "Weapon")
        {
            weaponFilter.style.display = DisplayStyle.Flex;
            armourFilter.style.display = DisplayStyle.None;

            weapons = FindAllWeapons();

            if (weaponFilter.value != "All")
                gears = weapons.Where(data => data.WeaponType.ToString() == weaponFilter.value).Cast<GearData>().ToList();
            else
                gears = weapons.Cast<GearData>().ToList();
        }
        else if (dropdownField.value == "Armour")
        {
            weaponFilter.style.display = DisplayStyle.None;
            armourFilter.style.display = DisplayStyle.Flex;

            armours = FindAllArmour();

            if (armourFilter.value != "All")
                gears = armours.Where(data => data.ArmourSlot.ToString() == armourFilter.value).Cast<GearData>().ToList();
            else
                gears = armours.Cast<GearData>().ToList();
        }

        List<string> pathList = gears.Select(data => AssetDatabase.GUIDToAssetPath(data.Guid)).ToList();
        SetupListView(gears, pathList, "gear-list");

        list.selectionChanged += (enumerable) =>
        {
            if (isInitialized)
                SessionState.SetInt("gearListIndex", list.selectedIndex); 

            foreach (UnityEngine.Object it in enumerable)
            {
                Box dataInfoBox = rootVisualElement.Query<Box>("gear-info").First();
                dataInfoBox.Clear();

                Box objectPreview = rootVisualElement.Query<Box>("object-preview").First();
                objectPreview.Clear();

                GearData data = it as GearData;

                if (data == null) return;

                SerializedObject serializeGear = new SerializedObject(data);
                SerializedProperty dataProperty = serializeGear.GetIterator();
                dataProperty.Next(true);

                while (dataProperty.NextVisible(false))
                {
                    PropertyField prop = new PropertyField(dataProperty);

                    prop.SetEnabled(dataProperty.name != "m-Script");
                    prop.Bind(serializeGear);
                    dataInfoBox.Add(prop);

                    // Update prefab
                    if (dataProperty.name == "Prefab")
                    {
                        prop.RegisterCallback<ChangeEvent<UnityEngine.Object>>((changeEvt) => LoadPrefab(data));
                    }
                }

                LoadPrefab(data);
            }
        };

        list.Rebuild();

        if (!isInitialized) 
            list.SetSelection(listIndex);
    }

    public override void SetButtons()
    {
        base.SetButtons();

        Button refreshButton = rootVisualElement.Query<Button>("refresh").First();
        refreshButton.clicked += RefreshScripts; 
    }

    public override void AddButton()
    {
        window = CreateInstance<GearPopupWindow>();
        window.addButtonPressed = true;
        isPopupActive = true;
        window.window = this;

        Vector2 mousePos = GUIUtility.GUIToScreenPoint(UnityEngine.Event.current.mousePosition);
        window.position = new Rect(mousePos.x, mousePos.y, 300, 250);
        window.ShowPopup();
    }

    public override void DeleteButton()
    {
        if (list.selectedItem != null)
        {
            GearData selectedGear = list.selectedItem as GearData;
            if (!EditorUtility.DisplayDialog($"Delete Gear", $"Delete {selectedGear.name}?", "Delete", "Cancel"))
                return;

            list.ClearSelection();
            rootVisualElement.Query<Box>("gear-info").First().Clear();
            list.itemsSource = null;

            AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(selectedGear.Guid));

            CreateListView();

            Box gameObjectPreview = rootVisualElement.Query<Box>("object-preview").First();
            gameObjectPreview.Clear();
        }
    }

    public override void RenameButton()
    {
        if (list.selectedItem != null)
        {
            window = CreateInstance<GearPopupWindow>();
            window.renameButtonPressed = true;
            isPopupActive = true;
            window.window = this;

            Vector2 mousePos = GUIUtility.GUIToScreenPoint(UnityEngine.Event.current.mousePosition);
            window.position = new Rect(mousePos.x, mousePos.y, 300, 200);
            window.ShowPopup();
        }
    }

    private void RefreshScripts()
    {
        EditorUtility.RequestScriptReload();
    }

    private List<GearData> FindAllGears()
    {
        string[] guids = AssetDatabase.FindAssets("t:GearData");

        List<GearData> gears = new List<GearData>();

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);

            GearData loadedData = AssetDatabase.LoadAssetAtPath<GearData>(path);
            loadedData.Guid = guids[i];

            gears.Add(loadedData);
        }

        return gears;
    }

    private List<WeaponData> FindAllWeapons()
    {
        string[] guids = AssetDatabase.FindAssets("t:WeaponData");

        List<WeaponData> weapons = new List<WeaponData>();

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);

            WeaponData loadedData = AssetDatabase.LoadAssetAtPath<WeaponData>(path);
            loadedData.Guid = guids[i];

            weapons.Add(loadedData);
        }

        return weapons;
    }

    private List<ArmourData> FindAllArmour()
    {
        string[] guids = AssetDatabase.FindAssets("t:ArmourData");

        List<ArmourData> armours = new List<ArmourData>();

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);

            ArmourData loadedData = AssetDatabase.LoadAssetAtPath<ArmourData>(path);
            loadedData.Guid = guids[i];

            armours.Add(loadedData);
        }

        return armours;
    }

    private void LoadPrefab(GearData data)
    {
        if (data.Prefab == null)
            return;

        Box gameObjectPreview = rootVisualElement.Query<Box>("object-preview").First();
        gameObjectPreview.Clear();

        GUIStyle bgColor = new GUIStyle();
        bgColor.normal.background = EditorGUIUtility.whiteTexture;

        if (isInitialized)
            DestroyImmediate(gameObjectEditor);

        gameObjectEditor = Editor.CreateEditor(data.Prefab);
        IMGUIContainer container = new IMGUIContainer(() => { gameObjectEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(1000, 500), bgColor); });
        gameObjectPreview.Add(container);
    }

}
