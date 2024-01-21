using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using static UnityEngine.GraphicsBuffer;
using UnityEditor.PackageManager.UI;

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

        EditorStyles.label.wordWrap = true;

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

                    if (cardProperty.name == "image" || cardProperty.name == "frame")
                    {
                        prop.RegisterCallback<ChangeEvent<Object>>((changeEvt) => LoadCardImage(card));
                    }

                    if (cardProperty.name == "name" || cardProperty.name == "description" || cardProperty.name == "flavour")
                    {
                        prop.RegisterValueChangeCallback(changeEvt => LoadCardText(card));

                        //prop.RegisterCallback<ChangeEvent<Label>>((changeEvt) => LoadCardText(card));
                    }
                }

                LoadCardImage(card);
                LoadCardText(card);
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

    private void LoadCardImage(Card card)
    {
        Image cardPreviewImage = rootVisualElement.Query<Image>("preview").First();
        Image cardPreviewFrame = rootVisualElement.Query<Image>("preview2").First();
        cardPreviewImage.image = card.image.texture;
        cardPreviewFrame.image = card.frame.texture;
    }

    private void LoadCardText(Card card)
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


    static Texture2D GetPrefabPreview(string path)
    {

        Debug.Log("Generate preview for " + path);
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        Texture2D tex = AssetPreview.GetAssetPreview(prefab);


        //var editor = Editor.CreateEditor(prefab);
        //Texture2D tex = editor.RenderStaticPreview(path, null, 200, 200);
        //EditorWindow.DestroyImmediate(editor);

        return tex;
    }
}
