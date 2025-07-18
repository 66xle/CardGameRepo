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
using UnityEngine.Assertions.Must;
using System.Linq;

public class CardEditorWindow : BaseEditorWindow
{
    private Card selectedCard;
    private string lastDisplayDescription;

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

        EditorApplication.update += UpdateCardUI;

        EditorApplication.delayCall += () => { base.Init(); };
    }

    private void OnDisable()
    {
        EditorApplication.update -= UpdateCardUI;
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
                Box cardInfoBox = rootVisualElement.Query<Box>("card-info").First();
                cardInfoBox.Clear();

                Card card = it as Card;
                selectedCard = card;

                if (card == null) return;

                SerializedObject serializeCard = new SerializedObject(card);
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
                        prop.RegisterCallback<ChangeEvent<UnityEngine.Object>>((changeEvt) => LoadCardImage(card));
                    }

                    if (cardProperty.name == "CardName" || cardProperty.name == "Description" ||
                        cardProperty.name == "Flavour" || cardProperty.name == "Value" || cardProperty.name == "Cost")
                    {
                        prop.RegisterValueChangeCallback(changeEvt => LoadCardText(card, list));
                    }

                    if (cardProperty.name == "DisplayDescription")
                    {
                        lastDisplayDescription = card.DisplayDescription;
                    }

                }

                LoadCardImage(card);
                LoadCardText(card);
            }
        };

        list.Rebuild();

        if (!isInitialized)
            list.SetSelection(listIndex);
    }

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

    private void LoadCardImage(Card card)
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

    private void LoadCardText(Card card, ListView cardList = null)
    {
        Label title = rootVisualElement.Query<Label>("title").First();
        Label description = rootVisualElement.Query<Label>("description").First();
        Label flavour = rootVisualElement.Query<Label>("flavour").First();
        Label cost = rootVisualElement.Query<Label>("cost").First();

        CreateClickableText(card);

        title.text = card.CardName;
        description.text = card.LinkDescription;
        flavour.text = card.Flavour;
        cost.text = card.Cost.ToString();
    }

    private void UpdateCardUI()
    {
        if (selectedCard == null) return;

        if (selectedCard.DisplayDescription != lastDisplayDescription)
        {
            lastDisplayDescription = selectedCard.DisplayDescription;
            LoadCardText(selectedCard, list);
        }
    }

    private void CreateClickableText(Card card)
    {
        if (card.PopupKeyPair == null) 
            card.PopupKeyPair = new();

        card.PopupKeyPair.Clear();

        card.LinkDescription = card.Description;

        FindAllPopupText(out List<PopupText> popupList);

        foreach (PopupText popupText in popupList)
        {
            card.LinkDescription = card.LinkDescription.Replace($"#{popupText.Title}", $"<link=\"{popupText.Title}\"><color=#FFBF00><u>{popupText.Title}</u></color></link>");
            card.PopupKeyPair.Add(new SerializableKeyValuePair<string, PopupText>(popupText.Title, popupText));
        }
    }
}
