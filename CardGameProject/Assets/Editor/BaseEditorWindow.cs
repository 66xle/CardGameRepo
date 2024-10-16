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

        EditorStyles.label.wordWrap = true;
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
