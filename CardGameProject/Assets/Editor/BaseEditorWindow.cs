using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.UIElements;
using System.Web;
using System.IO;

public class BaseEditorWindow : EditorWindow
{
    protected static int listIndex;
    protected static bool isInitialized;
    protected static bool editorReadyToInit;

    public ListView list;

    public PopupWindow window;

    public bool isPopupActive;

    public string type;
    public string typeName;

    private void OnFocus()
    {
        if (isPopupActive)
        {
            window.Focus();
            EditorUtility.DisplayDialog($"Error", $"Currently creating {type}", "Ok");
        }
    }

    private void OnEnable()
    {
        Init();
    }

    private void OnGUI()
    {
        if (!isInitialized && editorReadyToInit)
        {
            rootVisualElement.Clear();
            Init();
        }
    }

    public virtual void Init()
    {
        CreateListView();
        SetButtons();

        isInitialized = true;
    }

    public static void ShowWindow(BaseEditorWindow window, string windowName)
    {
        window.titleContent = new GUIContent(windowName);
        window.minSize = new Vector2(800, 600);
    }

    public void Enable(string uxmlFileName, string stylesFileName, string type, string typeName)
    {
        this.type = type;
        this.typeName = typeName;

        VisualTreeAsset original = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"Assets/Editor/{typeName}/{uxmlFileName}.uxml");
        TemplateContainer treeAsset = original.CloneTree();
        rootVisualElement.Add(treeAsset);

        StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>($"Assets/Editor/{typeName}/{stylesFileName}.uss");
        rootVisualElement.styleSheets.Add(styleSheet);


    }

    public void SetupListView<T>(List<T> data, List<string> pathList, string listViewName)
    {
        list = rootVisualElement.Query<ListView>(listViewName).First();

        list.makeItem = () =>
        {
            VisualElement visualElement = new VisualElement();
            visualElement.AddToClassList("list-item");

            Label label = new Label();
            label.name = "list-item";

            Box border = new Box();
            border.AddToClassList("border");

            visualElement.Add(label);
            visualElement.Add(border);
            return visualElement;
        };

        list.bindItem = (element, i) =>
        {
            Label label = element.Query<Label>("list-item");

            if (i < pathList.Count) // Index error for some dumb reason
                label.text = Path.GetFileNameWithoutExtension(pathList[i]);
        };

        if (data != null)
            list.itemsSource = data;

        list.fixedItemHeight = 32;
        list.selectionType = SelectionType.Single;
    }


    public virtual void CreateListView() { }


    public virtual void SetButtons()
    {
        Button addButton = rootVisualElement.Query<Button>($"add-{type}").First();
        addButton.clicked += AddButton;

        Button deleteButton = rootVisualElement.Query<Button>($"delete-{type}").First();
        deleteButton.clicked += DeleteButton;

        Button renameButton = rootVisualElement.Query<Button>($"rename-{type}").First();
        renameButton.clicked += RenameButton;
    }

    public virtual void AddButton() { }

    public virtual void DeleteButton() { }

    public virtual void RenameButton() { }

}
