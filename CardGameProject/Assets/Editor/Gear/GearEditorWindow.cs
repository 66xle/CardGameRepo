using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

using Button = UnityEngine.UIElements.Button;
using Image = UnityEngine.UIElements.Image;

public class GearEditorWindow : BaseEditorWindow
{
    GameObject gameObject;
    Editor gameObjectEditor;


    Button detailButton;
    Button cardButton;
    Button animationButton;

    GroupBox detailContent;
    GroupBox cardContent;
    GroupBox animationContent;
    VisualElement objectPreview;
    VisualElement cardPreview;

    private readonly Dictionary<int, GearCardElement> cardElements = new();
    ListView cardList;
    SerializedObject selectedObj;

    private EventCallback<ChangeEvent<UnityEngine.Object>> _objCallback;
    private EventCallback<ChangeEvent<int>> _intCallback;


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

            cardList = rootVisualElement.Query<ListView>("cards-list").First();

            objectPreview = rootVisualElement.Query<Box>("object-preview").First();
            cardPreview = rootVisualElement.Query<Box>("card-preview").First();

            #region Filters
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
            #endregion

            #region Tabs
            detailContent = rootVisualElement.Query<GroupBox>("details").First();
            cardContent = rootVisualElement.Query<GroupBox>("cards").First();
            animationContent = rootVisualElement.Query<GroupBox>("animations").First();

            detailButton = rootVisualElement.Query<Button>("detail-tab").First();
            cardButton = rootVisualElement.Query<Button>("card-tab").First();
            animationButton = rootVisualElement.Query<Button>("animation-tab").First();

            #region Button Hover Callbacks

            detailButton.RegisterCallback<MouseEnterEvent>(evt =>
            {
                if (detailButton.enabledSelf)
                    detailButton.style.backgroundColor = new StyleColor(new Color32(103, 103, 103, 255));
            });

            detailButton.RegisterCallback<MouseLeaveEvent>(evt =>
            {
                if (detailButton.enabledSelf)
                    detailButton.style.backgroundColor = new StyleColor(new Color32(88, 88, 88, 255));
            });

            cardButton.RegisterCallback<MouseEnterEvent>(evt =>
            {
                if (cardButton.enabledSelf)
                    cardButton.style.backgroundColor = new StyleColor(new Color32(103, 103, 103, 255));
            });

            cardButton.RegisterCallback<MouseLeaveEvent>(evt =>
            {
                if (cardButton.enabledSelf)
                    cardButton.style.backgroundColor = new StyleColor(new Color32(88, 88, 88, 255));
            });

            animationButton.RegisterCallback<MouseEnterEvent>(evt =>
            {
                if (animationButton.enabledSelf)
                    animationButton.style.backgroundColor = new StyleColor(new Color32(103, 103, 103, 255));
            });

            animationButton.RegisterCallback<MouseLeaveEvent>(evt =>
            {
                if (animationButton.enabledSelf)
                    animationButton.style.backgroundColor = new StyleColor(new Color32(88, 88, 88, 255));
            });

            #endregion

            detailButton.clicked += DetailTab;
            cardButton.clicked += CardTab;
            animationButton.clicked += AnimationTab;

            #endregion

            Button addCardButton = rootVisualElement.Query<Button>("cards-add-button").First();
            Button removeCardButton = rootVisualElement.Query<Button>("cards-remove-button").First();

            StyleColor hover = new StyleColor(new Color32(63, 63, 63, 255));
            StyleColor normal = new StyleColor(new Color32(51, 51, 51, 255));

            addCardButton.RegisterCallback<MouseEnterEvent>(evt => addCardButton.style.backgroundColor = hover);
            addCardButton.RegisterCallback<MouseLeaveEvent>(evt => addCardButton.style.backgroundColor = normal);
            
            removeCardButton.RegisterCallback<MouseEnterEvent>(evt => removeCardButton.style.backgroundColor = hover);
            removeCardButton.RegisterCallback<MouseLeaveEvent>(evt => removeCardButton.style.backgroundColor = normal);
        };
    }

    public override void CreateListView()
    {
        #region Filter & Gear Setup

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

        #endregion

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

                if (detailButton != null || cardButton != null || animationButton != null)
                {
                    if (!cardButton.enabledSelf)
                    {
                        CardTab();
                    }
                    else if (!animationButton.enabledSelf)
                    {
                        AnimationTab();
                    }
                    else
                    {
                        DetailTab();
                        LoadPrefab(data);
                    }
                }
            }
        };

        list.Rebuild();

        if (!isInitialized)
            list.SetSelection(listIndex);
    }

    public void LoadDetailContent(GearData data, SerializedObject obj)
    {
        TextField name = rootVisualElement.Query<TextField>("detail-name").First();
        TextField description = rootVisualElement.Query<TextField>("detail-description").First();
        DropdownField rarity = rootVisualElement.Query<DropdownField>("detail-rarity").First();
        ObjectField prefab = rootVisualElement.Query<ObjectField>("detail-prefab").First();
        ObjectField icon = rootVisualElement.Query<ObjectField>("detail-icon").First();

        name.Bind(obj);
        description.Bind(obj);
        rarity.Bind(obj);
        prefab.Bind(obj);
        icon.Bind(obj);

        GroupBox weaponBox = rootVisualElement.Query<GroupBox>("detail-weapon-box").First();
        GroupBox armourBox = rootVisualElement.Query<GroupBox>("detail-armour-box").First();

        // check weapon or armour
        if (data is WeaponData)
        {
            weaponBox.style.display = DisplayStyle.Flex;
            armourBox.style.display = DisplayStyle.None;

            DropdownField damageType = rootVisualElement.Query<DropdownField>("detail-damage-type").First();
            DropdownField weaponType = rootVisualElement.Query<DropdownField>("detail-weapon-type").First();
            TextField weaponAttack = rootVisualElement.Query<TextField>("detail-weapon-attack").First();

            damageType.Bind(obj);
            weaponType.Bind(obj);
            weaponAttack.Bind(obj);
        }
        else
        {
            armourBox.style.display = DisplayStyle.Flex;
            weaponBox.style.display = DisplayStyle.None;

            DropdownField armourSlot = rootVisualElement.Query<DropdownField>("detail-armour-slot").First();
            TextField armourDefence = rootVisualElement.Query<TextField>("detail-armour-defence").First();

            armourSlot.Bind(obj);
            armourDefence.Bind(obj);
        }
    }

    public void LoadCardContent(GearData data, SerializedObject obj)
    {
        if (cardList == null) return;

        rootVisualElement.Query<VisualElement>("card-options").First().style.display = DisplayStyle.None;

        cardElements.Clear();
        selectedObj = obj;
        cardList.selectionChanged -= OnCardSelectionChanged;
        cardList.Clear();
        cardList.selectedIndex = -1;

        cardList.reorderable = false;

        if (data.Cards.Count == 0)
        {
            if (cardList.itemsSource == null) return;

            cardList.itemsSource.Clear();
            return;
        }

        cardList.makeItem = () =>
        {
            GearCardElement gearCardElement = new GearCardElement();
            return gearCardElement;
        };

        cardList.bindItem = (element, i) =>
        {
            GearCardElement gearCardElement = element as GearCardElement;
            cardElements[i] = gearCardElement;
            gearCardElement.Selected = i == cardList.selectedIndex;

            Label label = element.Query<Label>($"card-title");

            if (i < data.Cards.Count)
            {
                if (data.Cards[i].Card == null)
                {
                    label.text = "<Missing Card>";
                    gearCardElement.Icon = null;
                }
                else
                {
                    label.text = data.Cards[i].Card.CardName;
                    gearCardElement.Icon = data.Cards[i].Card.Image.texture;
                }
            }
        };

        if (data != null)
            cardList.itemsSource = data.Cards;

        cardList.selectionChanged += OnCardSelectionChanged;

    }

    public void OnCardSelectionChanged(IEnumerable<object> enumerable)
    {
        rootVisualElement.Query<VisualElement>("card-options").First().style.display = DisplayStyle.Flex;

        SerializedProperty prop = selectedObj.FindProperty("_cards");
        SerializedProperty cardProperty = prop.GetArrayElementAtIndex(cardList.selectedIndex);


        SerializedProperty cardProp = cardProperty.FindPropertyRelative("Card");
        SerializedProperty amountProp = cardProperty.FindPropertyRelative("CardAmount");

        
        ObjectField cardField = rootVisualElement.Query<ObjectField>("card-object").First();
        cardField.Unbind();
        cardField.BindProperty(cardProp);

        if (_objCallback != null)
            cardField.UnregisterValueChangedCallback(_objCallback);

        cardField.RegisterValueChangedCallback(_objCallback = evt => ObjCallBack(cardProp, evt));


        IntegerField countField = rootVisualElement.Query<IntegerField>("card-amount").First();
        countField.Unbind();
        countField.BindProperty(amountProp);

        if (_intCallback != null)
            countField.UnregisterValueChangedCallback(_intCallback);

        countField.RegisterValueChangedCallback(_intCallback = evt => IntCallBack(amountProp, evt));

        cardList.RefreshItems();

        CardAnimationData animationData = cardList.selectedItem as CardAnimationData;
        if (animationData.Card == null)
        {
            ClearCardPreview();
            return;
        }

        LoadCardImage(animationData.Card);
        LoadCardText(animationData.Card);
    }

    private void ObjCallBack(SerializedProperty prop, ChangeEvent<UnityEngine.Object> evt)
    {
        prop.objectReferenceValue = evt.newValue;
        selectedObj.ApplyModifiedProperties();
        cardList.RefreshItems();
    }

    private void IntCallBack(SerializedProperty prop, ChangeEvent<int> evt)
    {
        prop.intValue = evt.newValue;
        selectedObj.ApplyModifiedProperties();
    }

    private void LoadCardImage(Card card, CardVariant variant = null)
    {
        Image cardPreviewImage = rootVisualElement.Query<Image>("preview").First();
        Image cardPreviewFrame = rootVisualElement.Query<Image>("preview2").First();

        try
        {
            cardPreviewImage.image = card.Image.texture;
        }
        catch (Exception err)
        {
            cardPreviewImage.image = null;
        }

        try
        {
            cardPreviewFrame.image = card.Frame.texture;
        }
        catch (Exception err)
        {
            cardPreviewFrame.image = null;
        }
    }

    private void LoadCardText(Card card, CardVariant variant = null)
    {
        Label title = rootVisualElement.Query<Label>("title").First();
        Label description = rootVisualElement.Query<Label>("description").First();
        Label flavour = rootVisualElement.Query<Label>("flavour").First();
        Label cost = rootVisualElement.Query<Label>("cost").First();

        //CardVariant IsVariantNull = null;

        //if (variant != null)
        //    IsVariantNull = variant.OverrideDescription ? variant : null;

        //CreateClickableText(card, IsVariantNull);

        title.text = card.CardName;
        description.text = card.LinkDescription;
        flavour.text = card.Flavour;
        cost.text = card.Cost.ToString();
    }

    private void ClearCardPreview()
    {
        Image cardPreviewImage = rootVisualElement.Query<Image>("preview").First();
        Image cardPreviewFrame = rootVisualElement.Query<Image>("preview2").First();

        Label title = rootVisualElement.Query<Label>("title").First();
        Label description = rootVisualElement.Query<Label>("description").First();
        Label flavour = rootVisualElement.Query<Label>("flavour").First();
        Label cost = rootVisualElement.Query<Label>("cost").First();

        cardPreviewImage.image = null;
        cardPreviewFrame.image = null;
        title.text = "";
        description.text = "";
        flavour.text = "";
        cost.text = "";
    }

    public void DetailTab()
    {
        ActiveTab(detailButton);
        DeactiveTab(cardButton);
        DeactiveTab(animationButton);
        detailContent.style.display = DisplayStyle.Flex;
        cardContent.style.display = DisplayStyle.None;
        animationContent.style.display = DisplayStyle.None;
        objectPreview.style.display = DisplayStyle.Flex;
        cardPreview.style.display = DisplayStyle.None;

        LoadDetailContent(list.selectedItem as GearData, new SerializedObject(list.selectedItem as GearData));
    }

    public void CardTab()
    {
        DeactiveTab(detailButton);
        ActiveTab(cardButton);
        DeactiveTab(animationButton);
        detailContent.style.display = DisplayStyle.None;
        cardContent.style.display = DisplayStyle.Flex;
        animationContent.style.display = DisplayStyle.None;
        objectPreview.style.display = DisplayStyle.None;
        cardPreview.style.display = DisplayStyle.Flex;

        LoadCardContent(list.selectedItem as GearData, new SerializedObject(list.selectedItem as GearData));
        ClearCardPreview();
    }

    public void AnimationTab()
    {
        DeactiveTab(detailButton);
        DeactiveTab(cardButton);
        ActiveTab(animationButton);
        detailContent.style.display = DisplayStyle.None;
        cardContent.style.display = DisplayStyle.None;
        animationContent.style.display = DisplayStyle.Flex;
        objectPreview.style.display = DisplayStyle.None;
        cardPreview.style.display = DisplayStyle.None;
    }

    public void ActiveTab(Button button)
    {
        button.style.backgroundColor = new StyleColor(new Color32(51, 51, 51, 255));
        button.SetEnabled(false);
        button.style.borderBottomWidth = 1;
    }

    public void DeactiveTab(Button button)
    {
        button.style.backgroundColor = new StyleColor(new Color32(88, 88, 88, 255));
        button.SetEnabled(true);
        button.style.borderBottomWidth = 0;
    }


    public override void SetButtons()
    {
        base.SetButtons();

        Button refreshButton = rootVisualElement.Query<Button>("refresh").First();
        refreshButton.clicked += RefreshScripts;
    }

    public override void AddButton()
    {
        popupWindow = CreateInstance<GearPopupWindow>();
        popupWindow.addButtonPressed = true;
        isPopupActive = true;
        popupWindow.window = this;

        Vector2 mousePos = GUIUtility.GUIToScreenPoint(UnityEngine.Event.current.mousePosition);
        popupWindow.position = new Rect(mousePos.x, mousePos.y, 300, 250);
        popupWindow.ShowPopup();
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
            popupWindow = CreateInstance<GearPopupWindow>();
            popupWindow.renameButtonPressed = true;
            isPopupActive = true;
            popupWindow.window = this;

            Vector2 mousePos = GUIUtility.GUIToScreenPoint(UnityEngine.Event.current.mousePosition);
            popupWindow.position = new Rect(mousePos.x, mousePos.y, 300, 200);
            popupWindow.ShowPopup();
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
