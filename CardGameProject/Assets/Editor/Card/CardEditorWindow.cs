using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class CardEditorWindow : BaseEditorWindow
{
    private Card selectedCard;
    private CardVariant selectedVariant;
    Box cardInfoBox;
    DynamicDropdown variantDropdown;

    [MenuItem("Editor/Card Editor")]
    public static void ShowWindow()
    {
        CardEditorWindow window = GetWindow<CardEditorWindow>();
        ShowWindow(window, "Card Editor");
    }

    [InitializeOnLoadMethod]
    private static void OnLoad()
    {
        listIndex = SessionState.GetInt("cardListIndex", 0);
        isInitialized = false;
        editorReadyToInit = true;
    }

    public override void Init()
    {
        Enable("CardEditorWindow", "CardEditorStyles", "card", "Card");

        EditorApplication.delayCall += () => { base.Init(); };

        cardInfoBox = rootVisualElement.Query<Box>("card-info").First();
    }

    public override void CreateListView()
    {
        FindAllCards(out List<Card> cards);

        if (cards.Count == 0) return;

        List<string> pathList = cards.Select(data => AssetDatabase.GUIDToAssetPath(data.Guid)).ToList();
        SetupListView(cards, pathList, "card-list");

        list.selectionChanged += (enumerable) =>
        {
            foreach (UnityEngine.Object it in enumerable)
            {
                cardInfoBox.Clear();

                Card card = it as Card;
                selectedCard = card;

                if (selectedCard == null) return;

                variantDropdown = rootVisualElement.Query<DynamicDropdown>("variant-dropdown").First();
                variantDropdown.ClearItems();
                variantDropdown.SetEnabled(true);
                variantDropdown.style.opacity = 1f;

                foreach (CardVariant variant in selectedCard.Variants)
                {
                    variantDropdown.AddItemExternal(variant.Name);
                }

                
                variantDropdown.OnItemSelected -= CheckCardVariant; 
                variantDropdown.OnItemSelected += CheckCardVariant; 
                variantDropdown.OnItemAdded -= AddCardVariant;
                variantDropdown.OnItemAdded += AddCardVariant;
                variantDropdown.OnItemDeleted -= DeleteCardVariant;
                variantDropdown.OnItemDeleted += DeleteCardVariant;

                LoadDefaultCard();
            }
        };

        list.Rebuild();

        if (!isInitialized)
            list.SetSelection(listIndex);
    }

    private void AddCardVariant(string name)
    {
        selectedCard.Variants.Add(new CardVariant(selectedCard) { Name = name });
    }

    private void DeleteCardVariant(string name)
    {
        int index = selectedCard.Variants.FindIndex(x => x.Name == name);
        if (index >= 0)
        {
            selectedCard.Variants.RemoveAt(index);
        }
    }

    private void CheckCardVariant(string name)
    {
        if (name == "Base Card")
        {
            LoadDefaultCard();
        }
        else
        {
            LoadCardVariant(name);
        }
    }

    private void LoadDefaultCard()
    {
        cardInfoBox.Clear();

        SerializedObject serializeCard = new SerializedObject(selectedCard);
        SerializedProperty cardProperty = serializeCard.GetIterator();
        cardProperty.Next(true);

        while (cardProperty.NextVisible(false))
        {
            PropertyField prop = new PropertyField(cardProperty);

            prop.SetEnabled(cardProperty.name != "m-Script");
            prop.Bind(serializeCard);
            cardInfoBox.Add(prop);

            // Update images and text
            if (cardProperty.name == "Image" || cardProperty.name == "Frame")
            {
                prop.RegisterCallback<ChangeEvent<UnityEngine.Object>>((changeEvt) => LoadCardImage(selectedCard));
            }

            if (cardProperty.name == "CardName" || cardProperty.name == "Description" ||
                cardProperty.name == "Flavour" || cardProperty.name == "Value" || cardProperty.name == "Cost")
            {
                prop.RegisterValueChangeCallback(changeEvt => LoadCardText(selectedCard));
            }

            if (cardProperty.name == "Variants")
            {
                prop.style.opacity = 0f;
                prop.style.height = 0;
            }

        }

        LoadCardImage(selectedCard);
        LoadCardText(selectedCard);
    }

    private void LoadCardVariant(string variant)
    {
        cardInfoBox.Clear();

        SerializedObject cardSO = new SerializedObject(selectedCard);
        SerializedProperty variants = cardSO.FindProperty("Variants");

        int index = selectedCard.Variants.FindIndex(x => x.Name == variant);
        SerializedProperty variantElement = variants.GetArrayElementAtIndex(index);

        selectedVariant = selectedCard.Variants[index];

        SerializedProperty variantProp = variantElement.Copy();

        SerializedProperty end = variantProp.GetEndProperty();
        bool enterChildren = true;

        PropertyField overRideDescription = null;
        PropertyField overRideFlavour = null;

        PropertyField overRideImage = null;
        PropertyField overRideFrame = null;

        PropertyField overRideCost = null;
        PropertyField overRideRecycleValue = null;

        PropertyField overRideCommands = null;


        while (variantProp.NextVisible(enterChildren) && !SerializedProperty.EqualContents(variantProp, end))
        {
            enterChildren = false;

            PropertyField prop = new PropertyField(variantProp);

            prop.SetEnabled(variantProp.name != "m-Script");
            prop.Bind(cardSO);

            #region Assign Override Properties

            if (variantProp.name == "OverrideDescription")
            {
                overRideDescription = prop;
            }
            else if (variantProp.name == "OverrideFlavour")
            {
                overRideFlavour = prop;
            }
            else if (variantProp.name == "OverrideImage")
            {
                overRideImage = prop;
            }
            else if (variantProp.name == "OverrideFrame")
            {
                overRideFrame = prop;
            }
            else if (variantProp.name == "OverrideCost")
            {
                overRideCost = prop;
            }
            else if (variantProp.name == "OverrideRecycleValue")
            {
                overRideRecycleValue = prop;
            }
            else if (variantProp.name == "OverrideCommands")
            {
                overRideCommands = prop;
            }

            #endregion


            if (variantProp.name == "Image" || variantProp.name == "Frame")
            {
                prop.RegisterValueChangeCallback(changeEvt => LoadCardImage(selectedCard, selectedVariant));

                if (variantProp.name == "Image")
                {
                    overRideImage.RegisterValueChangeCallback(evt => LoadCardImage(selectedCard, selectedVariant));
                }
                else if (variantProp.name == "Frame")
                {
                    overRideFrame.RegisterValueChangeCallback(evt => LoadCardImage(selectedCard, selectedVariant));
                }
            }

            if (variantProp.name == "CardName" || variantProp.name == "Value" || variantProp.name == "Cost")
            {
                prop.RegisterValueChangeCallback(changeEvt => LoadCardText(selectedCard, selectedVariant));
            }

            if (variantProp.name == "Description" || variantProp.name == "Flavour")
            {
                Label label = new Label(variantProp.name);
                label.style.marginTop = 10;

                TextField textArea = new TextField();
                textArea.RegisterCallback<AttachToPanelEvent>(evt =>
                {
                    var input = textArea.Q(TextField.textInputUssName);

                    if (input != null)
                    {
                        input.style.minHeight = 50;
                    }
                });

                textArea.multiline = true;
                textArea.value = variantProp.stringValue;
                VisualElement text = textArea.Query(className: "unity-text-element");
                

                if (variantProp.name == "Description")
                {
                    textArea.RegisterValueChangedCallback(evt =>
                    {
                        selectedVariant.Description = evt.newValue;
                        LoadCardText(selectedCard, selectedVariant);
                    });

                    overRideDescription.RegisterValueChangeCallback(evt =>
                    {
                        label.style.color = selectedVariant.OverrideDescription ? Color.white : Color.gray;

                        textArea.isReadOnly = selectedVariant.OverrideDescription ? false : true;
                        text.style.color = selectedVariant.OverrideDescription ? Color.white : Color.gray;

                        LoadCardText(selectedCard, selectedVariant);
                    });
                }
                else if (variantProp.name == "Flavour")
                {
                    textArea.RegisterValueChangedCallback(evt =>
                    {
                        selectedVariant.Flavour = evt.newValue;
                        LoadCardText(selectedCard, selectedVariant);
                    });

                    overRideFlavour.RegisterValueChangeCallback(evt =>
                    {
                        label.style.color = selectedVariant.OverrideFlavour ? Color.white : Color.gray;

                        textArea.isReadOnly = selectedVariant.OverrideFlavour ? false : true;
                        text.style.color = selectedVariant.OverrideFlavour ? Color.white : Color.gray;

                        LoadCardText(selectedCard, selectedVariant);
                    });
                }

                cardInfoBox.Add(label);
                cardInfoBox.Add(textArea);
                continue;
            }

            if (variantProp.name == "Image")
            {
                Label label = new Label("Card Image");
                label.style.unityFontStyleAndWeight = FontStyle.Bold;
                label.style.marginTop = 10;
                cardInfoBox.Add(label);
            }

            if (variantProp.name == "Cost")
            {
                Label label = new Label("Card Info");
                label.style.unityFontStyleAndWeight = FontStyle.Bold;
                label.style.marginTop = 10;

                cardInfoBox.Add(label);
            }


            
            cardInfoBox.Add(prop);
        }

        LoadCardImage(selectedCard, selectedVariant);
        LoadCardText(selectedCard, selectedVariant);
    }

    #region Buttons

    public override void SetButtons()
    {
        base.SetButtons();
    }

    public override void AddButton()
    {
        window = CreateInstance<CardPopupWindow>();
        window.addButtonPressed = true;
        isPopupActive = true;
        window.window = this;

        Vector2 mousePos = GUIUtility.GUIToScreenPoint(UnityEngine.Event.current.mousePosition);
        window.position = new Rect(mousePos.x, mousePos.y, 500, 700);
        window.ShowPopup();
    }

    public override void DeleteButton()
    {
        if (list.selectedItem != null)
        {
            Card selectedCard = list.selectedItem as Card;
            if (!EditorUtility.DisplayDialog($"Delete Card", $"Delete {selectedCard.CardName}?", "Delete", "Cancel"))
                return;

            list.ClearSelection();
            rootVisualElement.Query<Box>("card-info").First().Clear();
            list.itemsSource = null;

            AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(selectedCard.Guid));

            variantDropdown.SetEnabled(false);
            variantDropdown.style.opacity = 0f;

            CreateListView();

            #region Clear Text

            Label title = rootVisualElement.Query<Label>("title").First();
            Label description = rootVisualElement.Query<Label>("description").First();
            Label flavour = rootVisualElement.Query<Label>("flavour").First();
            Label cost = rootVisualElement.Query<Label>("cost").First();

            title.text = null;
            description.text = null;
            flavour.text = null;
            cost.text = null;

            #endregion
        }
    }

    public override void RenameButton()
    {
        if (list.selectedItem != null)
        {
            window = CreateInstance<CardPopupWindow>();
            window.renameButtonPressed = true;
            isPopupActive = true;
            window.window = this;

            Vector2 mousePos = GUIUtility.GUIToScreenPoint(UnityEngine.Event.current.mousePosition);
            window.position = new Rect(mousePos.x, mousePos.y, 300, 100);
            window.ShowPopup();
        }
    }

    #endregion

    public void FindAllCards(out List<Card> cards)
    {
        string[] guids = AssetDatabase.FindAssets("t:Card");

        cards = new List<Card>();

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);

            Card loadedCard = AssetDatabase.LoadAssetAtPath<Card>(path);
            loadedCard.Guid = guids[i];

            cards.Add(loadedCard);
        }
    }
    public void FindAllPopupText(out List<PopupText> popupList)
    {
        string[] guids = AssetDatabase.FindAssets("t:PopupText");

        popupList = new List<PopupText>();

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);

            PopupText loadedPopup = AssetDatabase.LoadAssetAtPath<PopupText>(path);

            popupList.Add(loadedPopup);
        }
    }

    private void LoadCardImage(Card card, CardVariant variant = null)
    {
        Image cardPreviewImage = rootVisualElement.Query<Image>("preview").First();
        Image cardPreviewFrame = rootVisualElement.Query<Image>("preview2").First();


        try
        {
            cardPreviewImage.image = (variant != null && variant.OverrideImage) ? variant.Image.texture : card.Image.texture;
        }
        catch (Exception err)
        {
            cardPreviewImage.image = null;
        }

        try
        {
            cardPreviewFrame.image = (variant != null && variant.OverrideFrame) ? variant.Frame.texture : card.Frame.texture;
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

        CardVariant IsVariantNull = null;

        if (variant != null)
            IsVariantNull = variant.OverrideDescription ? variant : null;

        CreateClickableText(card, IsVariantNull);

        title.text = card.CardName;
        description.text = (variant != null && variant.OverrideDescription) ? variant.LinkDescription : card.LinkDescription;
        flavour.text = (variant != null && variant.OverrideFlavour) ? variant.Flavour : card.Flavour;
        cost.text = (variant != null && variant.OverrideCost) ? variant.Cost.ToString() : card.Cost.ToString();
    }

    private void CreateClickableText(Card card, CardVariant variant = null)
    {
        FindAllPopupText(out List<PopupText> popupList);

        if (variant != null)
        {
            if (variant.PopupKeyPair == null)
                variant.PopupKeyPair = new();

            variant.PopupKeyPair.Clear();

            variant.LinkDescription = variant.Description;

            foreach (PopupText popupText in popupList)
            {
                variant.LinkDescription = variant.LinkDescription.Replace($"#{popupText.Title}", $"<link=\"{popupText.Title}\"><color=#FFBF00><u>{popupText.Title}</u></color></link>");
                variant.PopupKeyPair.Add(new SerializableKeyValuePair<string, PopupText>(popupText.Title, popupText));
            }

            return;
        }

        if (card.PopupKeyPair == null)
            card.PopupKeyPair = new();

        card.PopupKeyPair.Clear();

        card.LinkDescription = card.Description;

        foreach (PopupText popupText in popupList)
        {
            card.LinkDescription = card.LinkDescription.Replace($"#{popupText.Title}", $"<link=\"{popupText.Title}\"><color=#FFBF00><u>{popupText.Title}</u></color></link>");
            card.PopupKeyPair.Add(new SerializableKeyValuePair<string, PopupText>(popupText.Title, popupText));
        }
    }
}
