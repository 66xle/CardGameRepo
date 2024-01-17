using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class CardEditorWindow : EditorWindow
{
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

        CreateCardListView();
    }

    private void CreateCardListView()
    {
        FindAllCards(out Card[] cards);

        ListView cardList = rootVisualElement.Query<ListView>("card-list").First();
        cardList.makeItem = () => new Label();
        cardList.bindItem = (element, i) => (element as Label).text = cards[i].name;

        cardList.itemsSource = cards;
        cardList.itemHeight = 16;
        cardList.selectionType = SelectionType.Single;

        cardList.onSelectionChange += (enumerable) =>
        {
            foreach (Object it in enumerable)
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

                    if (cardProperty.name == "image")
                    {
                        prop.RegisterCallback<ChangeEvent<UnityEngine.Object>>((changeEvt) => LoadCardImage(card.image.texture));
                    }
                }

                LoadCardImage(card.image.texture);
            }
        };

        cardList.Refresh();
    }

    private void FindAllCards(out Card[] cards)
    {
        string[] guids = AssetDatabase.FindAssets("t:Card");

        cards = new Card[guids.Length];

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            cards[i] = AssetDatabase.LoadAssetAtPath<Card>(path);
        }
    }

    private void LoadCardImage(Texture texture)
    {
        Image cardPreviewImage = rootVisualElement.Query<Image>("preview").First();
        cardPreviewImage.image = texture;
    }
}
