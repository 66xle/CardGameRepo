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

public class CardEditorWindow : BaseEditorWindow
{
    [MenuItem("Editor/Card Editor")]
    public static void ShowWindow()
    {
        CardEditorWindow window = GetWindow<CardEditorWindow>();
        ShowWindow(window, "Card Editor");
    }

    private void OnEnable()
    {
        Enable("CardEditorWindow", "CardEditorStyles", "card", "Card");

        CreateListView();
        SetButtons();
    }

    public override void CreateListView()
    {
        FindAllCards(out List<Card> cards);

        list = rootVisualElement.Query<ListView>($"card-list").First();

        list.itemsSource = cards;

        list.bindItem = (element, i) =>
        {
            Label label = element.Q<Label>("list-item");
            label.text = Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(cards[i].guid));
        };

        SetupListView();

        list.selectionChanged += (enumerable) =>
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

                    if (cardProperty.name == "name" || cardProperty.name == "description" || 
                        cardProperty.name == "flavour" || cardProperty.name == "value" || cardProperty.name == "cost")
                    {
                        prop.RegisterValueChangeCallback(changeEvt => LoadCardText(card, list));
                    }
                }

                LoadCardImage(card);
                LoadCardText(card);
            }
        };

        list.Rebuild();
    }

    private void SetButtons()
    {
        Button addButton = rootVisualElement.Query<Button>($"add-{type}").First();
        addButton.clicked += AddCard;

        Button deleteButton = rootVisualElement.Query<Button>($"delete-{type}").First();
        deleteButton.clicked += DeleteCard;

        Button renameButton = rootVisualElement.Query<Button>($"rename-{type}").First();
        renameButton.clicked += RenameCard;
    }
    private void AddCard()
    {
        window = CreateInstance<CardPopupWindow>();
        window.addButtonPressed = true;
        isPopupActive = true;
        window.window = this;

        Vector2 mousePos = GUIUtility.GUIToScreenPoint(UnityEngine.Event.current.mousePosition);
        window.position = new Rect(mousePos.x, mousePos.y, 300, 400);
        window.ShowPopup();
    }

    private void DeleteCard()
    {
        if (list.selectedItem != null)
        {
            Card selectedCard = list.selectedItem as Card;
            if (!EditorUtility.DisplayDialog($"Delete Card", $"Delete {selectedCard.cardName}?", "Delete", "Cancel"))
                return;

            list.ClearSelection();
            rootVisualElement.Query<Box>("card-info").First().Clear();
            list.itemsSource = null;

            AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(selectedCard.guid));

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

    private void RenameCard()
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
        catch (Exception err) 
        {
            cardPreviewImage.image = null;
        }

        try
        {
            cardPreviewFrame.image = card.frame.texture;
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


        title.text = card.cardName;

        if (card.valuesToReference.Count == 0)
        {
            description.text = card.description.Replace("$value", "0");
        }
        else
        {
            description.text = card.description.Replace("$value", card.valuesToReference[0].ToString());
        }
            
        flavour.text = card.flavour;
        cost.text = card.cost.ToString();
    }

}
