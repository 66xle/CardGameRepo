using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using static UnityEngine.GraphicsBuffer;
using UnityEditor.PackageManager.UI;
using System;
using System.Security.Policy;
using System.Runtime.Remoting.Contexts;
using UnityEditor.Experimental.GraphView;
using System.IO;
using System.Runtime.CompilerServices;

public class WeaponEditorWindow : BaseEditorWindow
{

    GameObject gameObject;
    Editor gameObjectEditor;

    [MenuItem("Editor/Weapon Editor")]
    public static void ShowWindow()
    {
        WeaponEditorWindow window = GetWindow<WeaponEditorWindow>();
        ShowWindow(window, "Weapon Editor");
    }

    private void OnEnable()
    {
        Enable("WeaponEditorWindow", "WeaponEditorStyles", "weapon", "Weapon");

        CreateListView();
        SetButtons();
    }

    private void OnFocus()
    {
        if (isPopupActive)
        {
            window.Focus();
            EditorUtility.DisplayDialog($"Error", $"Currently creating weapon", "Ok");
        }
    }

    public override void CreateListView()
    {
        FindAllWeapons(out List<WeaponData> weapons);

        list = rootVisualElement.Query<ListView>("weapon-list").First();

        list.itemsSource = weapons;

        list.bindItem = (element, i) =>
        {
            Label label = element.Q<Label>("list-item");
            label.text = Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(weapons[i].Guid));
        };

        SetupListView();

        list.selectionChanged += (enumerable) =>
        {
            foreach (UnityEngine.Object it in enumerable)
            {
                Box weaponDataInfoBox = rootVisualElement.Query<Box>("weapon-info").First();
                weaponDataInfoBox.Clear();

                Box gameObjectPreview = rootVisualElement.Query<Box>("object-preview").First();
                gameObjectPreview.Clear();

                WeaponData weaponData = it as WeaponData;

                SerializedObject serializeWeapon = new SerializedObject(weaponData);
                SerializedProperty weaponDataProperty = serializeWeapon.GetIterator();
                weaponDataProperty.Next(true);

                while (weaponDataProperty.NextVisible(false))
                {
                    PropertyField prop = new PropertyField(weaponDataProperty);

                    prop.SetEnabled(weaponDataProperty.name != "m-Script");
                    prop.Bind(serializeWeapon);
                    weaponDataInfoBox.Add(prop);

                    // Update prefab
                    if (weaponDataProperty.name == "Prefab")
                    {
                        prop.RegisterCallback<ChangeEvent<UnityEngine.Object>>((changeEvt) => LoadWeaponPrefab(weaponData));
                    }
                }

                LoadWeaponPrefab(weaponData);
            }
        };

        list.Rebuild();
    }

    

    private void SetButtons()
    {
        Button addButton = rootVisualElement.Query<Button>("add-weapon").First();
        addButton.clicked += AddWeapon;
        
        Button deleteButton = rootVisualElement.Query<Button>("delete-weapon").First();
        deleteButton.clicked += DeleteWeapon;
        
        Button renameButton = rootVisualElement.Query<Button>("rename-weapon").First();
        renameButton.clicked += RenameWeapon;
    }
    private void AddWeapon()
    {
        window = CreateInstance<WeaponPopupWindow>();
        window.addButtonPressed = true;
        isPopupActive = true;
        window.window = this;

        Vector2 mousePos = GUIUtility.GUIToScreenPoint(UnityEngine.Event.current.mousePosition);
        window.position = new Rect(mousePos.x, mousePos.y, 300, 400);
        window.ShowPopup();
    }

    private void DeleteWeapon()
    {
        if (list.selectedItem != null)
        {
            WeaponData selectedWeapon = list.selectedItem as WeaponData;
            if (!EditorUtility.DisplayDialog($"Delete Weapon", $"Delete {selectedWeapon.name}?", "Delete", "Cancel"))
                return;

            list.ClearSelection();
            rootVisualElement.Query<Box>("weapon-info").First().Clear();
            list.itemsSource = null;

            AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(selectedWeapon.Guid));

            CreateListView();

            Box gameObjectPreview = rootVisualElement.Query<Box>("object-preview").First();
            gameObjectPreview.Clear();
        }
    }

    private void RenameWeapon()
    {
        if (list.selectedItem != null)
        {
            window = CreateInstance<WeaponPopupWindow>();
            window.renameButtonPressed = true;
            isPopupActive = true;
            window.window = this;

            Vector2 mousePos = GUIUtility.GUIToScreenPoint(UnityEngine.Event.current.mousePosition);
            window.position = new Rect(mousePos.x, mousePos.y, 300, 100);
            window.ShowPopup();
        }
    }

    private void FindAllWeapons(out List<WeaponData> weapons)
    {
        string[] guids = AssetDatabase.FindAssets("t:WeaponData");

        weapons = new List<WeaponData>();

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);

            WeaponData loadedWeaponData = AssetDatabase.LoadAssetAtPath<WeaponData>(path);
            loadedWeaponData.Guid = guids[i];

            weapons.Add(loadedWeaponData);
        }
    }

    private void LoadWeaponPrefab(WeaponData weaponData)
    {
        if (weaponData.Prefab == null)
            return;

        Box gameObjectPreview = rootVisualElement.Query<Box>("object-preview").First();
        gameObjectPreview.Clear();

        GUIStyle bgColor = new GUIStyle();
        bgColor.normal.background = EditorGUIUtility.whiteTexture;

        DestroyImmediate(gameObjectEditor);
        gameObjectEditor = Editor.CreateEditor(weaponData.Prefab);
        IMGUIContainer container = new IMGUIContainer(() => { gameObjectEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(1000, 500), bgColor); });
        gameObjectPreview.Add(container);
    }

}
