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
    public ListView list;

    public PopupWindow window;

    public bool isPopupActive;

    public string type;
    public string typeName;

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

    public void SetupListView()
    {
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
        
        list.fixedItemHeight = 32;
        list.selectionType = SelectionType.Single;
    }

    private void OnFocus()
    {
        if (isPopupActive)
        {
            window.Focus();
            EditorUtility.DisplayDialog($"Error", $"Currently creating {type}", "Ok");
        }
    }

    public virtual void CreateListView() { }

}
