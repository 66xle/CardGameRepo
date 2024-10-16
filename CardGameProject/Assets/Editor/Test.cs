using UnityEngine;
using UnityEditor;

// Create an editor window which can display a chosen GameObject.
// Use OnInteractivePreviewGUI to display the GameObject and
// allow it to be interactive.

public class ExampleClass : EditorWindow
{
    GameObject gameObject;
    Editor gameObjectEditor;

    [MenuItem("Example/GameObject Editor")]
    static void ShowWindow()
    {
        ExampleClass window = GetWindow<ExampleClass>();
        window.titleContent = new GUIContent("Preview");
        window.minSize = new Vector2(800, 600);
    }

    void OnGUI()
    {
        EditorGUI.BeginChangeCheck();


        gameObject = (GameObject)EditorGUILayout.ObjectField(gameObject, typeof(GameObject), true);

        if (EditorGUI.EndChangeCheck())
        {
            if (gameObjectEditor != null) 
                DestroyImmediate(gameObjectEditor);
        }


        GUIStyle bgColor = new GUIStyle();
        bgColor.normal.background = EditorGUIUtility.whiteTexture;


        if (gameObject != null)
        {
            if (gameObjectEditor == null)
                gameObjectEditor = Editor.CreateEditor(gameObject);

            gameObjectEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(256, 400), bgColor);
        }
    }
}