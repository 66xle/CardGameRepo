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

public class CardEditorWindow : EditorWindow
{
    public ListView cardList;

    private PopupWindow window;

    public bool isPopupActive;

    [MenuItem("Editor/Card Editor")]
    public static void ShowWindow()
    {
        CardEditorWindow window = GetWindow<CardEditorWindow>();
        window.titleContent = new GUIContent("Card Editor");
        window.minSize = new Vector2(800, 600);
    }

    private void OnEnable()
    {
        VisualTreeAsset original = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/Card/CardEditorWindow.uxml");
        TemplateContainer treeAsset = original.CloneTree();
        rootVisualElement.Add(treeAsset);

        StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/Card/CardEditorStyles.uss");
        rootVisualElement.styleSheets.Add(styleSheet);

        EditorStyles.label.wordWrap = true;

        CreateCardListView();
        SetButtons();
    }

    private void OnFocus()
    {
        if (isPopupActive)
        {
            window.Focus();
            EditorUtility.DisplayDialog($"Error", $"Currently creating card", "Ok");

            //window.Close();
            //Close();
        }
    }

    public void CreateCardListView()
    {
        FindAllCards(out List<Card> cards);

        cardList = rootVisualElement.Query<ListView>("card-list").First();
        cardList.makeItem = () => new Label();
        cardList.bindItem = (element, i) => (element as Label).text = Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(cards[i].guid));

        cardList.itemsSource = cards;
        cardList.itemHeight = 16;
        cardList.selectionType = SelectionType.Single;

        cardList.onSelectionChange += (enumerable) =>
        {
            foreach (UnityEngine.Object it in enumerable)
            {
                Box cardInfoBox = rootVisualElement.Query<Box>("card-info").First();
                cardInfoBox.Clear();

                Card card = it as Card;

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
                    if (cardProperty.name == "image" || cardProperty.name == "frame")
                    {
                        prop.RegisterCallback<ChangeEvent<UnityEngine.Object>>((changeEvt) => LoadCardImage(card));
                    }

                    if (cardProperty.name == "name" || cardProperty.name == "description" || cardProperty.name == "flavour")
                    {
                        prop.RegisterValueChangeCallback(changeEvt => LoadCardText(card, cardList));
                    }
                }

                LoadCardImage(card);
                LoadCardText(card);
            }
        };

        cardList.Refresh();
    }

    private void SetButtons()
    {
        Button addButton = rootVisualElement.Query<Button>("add-card").First();
        addButton.clicked += AddCard;

        Button deleteButton = rootVisualElement.Query<Button>("delete-card").First();
        deleteButton.clicked += DeleteCard;

        Button renameButton = rootVisualElement.Query<Button>("rename-card").First();
        renameButton.clicked += RenameCard;
    }
    private void AddCard()
    {
        window = CreateInstance<PopupWindow>();
        window.addCardButtonPressed = true;
        isPopupActive = true;
        window.Window = this;

        Vector2 mousePos = GUIUtility.GUIToScreenPoint(UnityEngine.Event.current.mousePosition);
        window.position = new Rect(mousePos.x, mousePos.y, 300, 400);
        window.ShowPopup();
    }

    private void DeleteCard()
    {
        if (cardList.selectedItem != null)
        {
            Card selectedCard = cardList.selectedItem as Card;
            if (!EditorUtility.DisplayDialog($"Delete Card", $"Delete {selectedCard.name}?", "Delete", "Cancel"))
                return;

            cardList.ClearSelection();
            rootVisualElement.Query<Box>("card-info").First().Clear();
            cardList.itemsSource = null;

            AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(selectedCard.guid));

            CreateCardListView();
        }
    }

    private void RenameCard()
    {
        if (cardList.selectedItem != null)
        {
            window = CreateInstance<PopupWindow>();
            window.renameCardButtonPressed = true;
            isPopupActive = true;
            window.Window = this;

            Vector2 mousePos = GUIUtility.GUIToScreenPoint(UnityEngine.Event.current.mousePosition);
            window.position = new Rect(mousePos.x, mousePos.y, 300, 100);
            window.ShowPopup();
        }
    }

    private void FindAllCards(out List<Card> cards)
    {
        string[] guids = AssetDatabase.FindAssets("t:Card");

        cards = new List<Card>();

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);

            Card loadedCard = AssetDatabase.LoadAssetAtPath<Card>(path);
            loadedCard.guid = guids[i];

            cards.Add(loadedCard);
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
        description.text = card.description;
        flavour.text = card.flavour;
        cost.text = card.cost.ToString();
    }

}
