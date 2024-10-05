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

public class WeaponEditorWindow : EditorWindow
{
    public ListView weaponDataList;

    GameObject gameObject;
    Editor gameObjectEditor;

    

    private WeaponPopupWindow window;

    public bool isPopupActive;

    [MenuItem("Editor/Weapon Editor")]
    public static void ShowWindow()
    {
        WeaponEditorWindow window = GetWindow<WeaponEditorWindow>();
        window.titleContent = new GUIContent("Weapon Editor");
        window.minSize = new Vector2(800, 600);
    }

    private void OnEnable()
    {
        VisualTreeAsset original = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/Weapon/WeaponEditorWindow.uxml");
        TemplateContainer treeAsset = original.CloneTree();
        rootVisualElement.Add(treeAsset);

        StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/Weapon/WeaponEditorStyles.uss");
        rootVisualElement.styleSheets.Add(styleSheet);

        EditorStyles.label.wordWrap = true;

        CreateWeaponListView();
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

    public void CreateWeaponListView()
    {
        FindAllWeapons(out List<WeaponData> weapons);

        weaponDataList = rootVisualElement.Query<ListView>("weapon-list").First();
        weaponDataList.makeItem = () => new Label();
        weaponDataList.bindItem = (element, i) => (element as Label).text = Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(weapons[i].guid));

        weaponDataList.itemsSource = weapons;
        weaponDataList.itemHeight = 16;
        weaponDataList.selectionType = SelectionType.Single;

        weaponDataList.onSelectionChange += (enumerable) =>
        {
            foreach (UnityEngine.Object it in enumerable)
            {
                Box weaponDataInfoBox = rootVisualElement.Query<Box>("weapon-info").First();
                weaponDataInfoBox.Clear();

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

                    Box gameObjectPreview = rootVisualElement.Query<Box>("object-preview").First();
                    gameObjectPreview.Clear();

                    GUIStyle bgColor = new GUIStyle();
                    bgColor.normal.background = EditorGUIUtility.whiteTexture;

                    DestroyImmediate(gameObjectEditor);
                    gameObjectEditor = Editor.CreateEditor(weaponData.prefab);
                    IMGUIContainer container = new IMGUIContainer(() => { gameObjectEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(1000, 500), bgColor); });
                    gameObjectPreview.Add(container);


                    // Update images and text
                    //if (weaponDataProperty.name == "image" || weaponDataProperty.name == "frame")
                    //{
                    //    prop.RegisterCallback<ChangeEvent<UnityEngine.Object>>((changeEvt) => LoadCardImage(weaponData));
                    //}
                    //
                    //if (weaponDataProperty.name == "name" || weaponDataProperty.name == "description" || 
                    //    weaponDataProperty.name == "flavour" || weaponDataProperty.name == "value" || weaponDataProperty.name == "cost")
                    //{
                    //    prop.RegisterValueChangeCallback(changeEvt => LoadCardText(weaponData, weaponDataList));
                    //}
                }

                //LoadCardImage(weaponData);
                //LoadCardText(weaponData);
            }
        };

        weaponDataList.Refresh();
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
        window.addWeaponButtonPressed = true;
        isPopupActive = true;
        window.Window = this;

        Vector2 mousePos = GUIUtility.GUIToScreenPoint(UnityEngine.Event.current.mousePosition);
        window.position = new Rect(mousePos.x, mousePos.y, 300, 400);
        window.ShowPopup();
    }

    private void DeleteWeapon()
    {
        if (weaponDataList.selectedItem != null)
        {
            WeaponData selectedWeapon = weaponDataList.selectedItem as WeaponData;
            if (!EditorUtility.DisplayDialog($"Delete Weapon", $"Delete {selectedWeapon.name}?", "Delete", "Cancel"))
                return;

            weaponDataList.ClearSelection();
            rootVisualElement.Query<Box>("weapon-info").First().Clear();
            weaponDataList.itemsSource = null;

            AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(selectedWeapon.guid));

            CreateWeaponListView();
        }
    }

    private void RenameWeapon()
    {
        if (weaponDataList.selectedItem != null)
        {
            window = CreateInstance<WeaponPopupWindow>();
            window.renameWeaponButtonPressed = true;
            isPopupActive = true;
            window.Window = this;

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
            loadedWeaponData.guid = guids[i];

            weapons.Add(loadedWeaponData);
        }
    }

    private void LoadCardImage(Card card)
    {
        Image cardPreviewImage = rootVisualElement.Query<Image>("preview").First();
        Image cardPreviewFrame = rootVisualElement.Query<Image>("preview2").First();

        try
        {
            cardPreviewImage.image = card.image.texture;
        }
        catch (Exception err) { }

        try
        {
            cardPreviewFrame.image = card.frame.texture;
        }
        catch (Exception err) { }
    }

    private void LoadCardText(Card card, ListView cardList = null)
    {
        Label title = rootVisualElement.Query<Label>("title").First();
        Label description = rootVisualElement.Query<Label>("description").First();
        Label flavour = rootVisualElement.Query<Label>("flavour").First();
        Label cost = rootVisualElement.Query<Label>("cost").First();

        title.text = card.name;
        description.text = card.description.Replace("$value", card.value.ToString());
        flavour.text = card.flavour;
        cost.text = card.cost.ToString();
    }

}
