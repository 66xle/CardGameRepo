using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelEditorWindow : BaseEditorWindow
{
    GameObject gameObject;
    Editor gameObjectEditor;

    [MenuItem("Editor/Level Editor")]
    public static void ShowWindow()
    {
        LevelEditorWindow window = GetWindow<LevelEditorWindow>();
        ShowWindow(window, "Level Editor");
    }

    [InitializeOnLoadMethod]
    private static void OnLoad()
    {
        listIndex = SessionState.GetInt("levelListIndex", 0);
        isInitialized = false;
        editorReadyToInit = true;
    }

    public override void Init()
    {
        Enable("LevelEditorWindow", "LevelEditorStyles", "level", "Level");

        EditorApplication.delayCall += () => 
        {
            base.Init();

            DropdownField dropdownField = rootVisualElement.Query<DropdownField>("filter");
            dropdownField.RegisterCallback<ChangeEvent<string>>((evt) =>
            {
                dropdownField.value = evt.newValue;
                CreateListView();
            });
        };
    }

    public override void CreateListView()
    {
        FindAllLevels(out List<LevelData> levels);

        
        DropdownField dropdownField = rootVisualElement.Query<DropdownField>("filter");
        
        if (dropdownField.value != "Any")
        {
            levels = levels.Where(data => data.IsFixed == true).ToList();
        }

        List<string> pathList = levels.Select(data => AssetDatabase.GUIDToAssetPath(data.Guid)).ToList();
        SetupListView(levels, pathList, "level-list");

        list.selectionChanged += (enumerable) =>
        {
            if (isInitialized)
                SessionState.SetInt("levelListIndex", list.selectedIndex); 

            foreach (UnityEngine.Object it in enumerable)
            {

                Box infoBox = rootVisualElement.Query<Box>("level-info").First();
                infoBox.Clear();

                Box preview = rootVisualElement.Query<Box>("object-preview").First();
                preview.Clear();

                LevelData data = it as LevelData;

                if (data == null) return;

                SerializedObject serializeObj = new SerializedObject(data);
                SerializedProperty dataProperty = serializeObj.GetIterator();
                dataProperty.Next(true);

                while (dataProperty.NextVisible(false))
                {
                    PropertyField prop = new PropertyField(dataProperty);

                    prop.SetEnabled(dataProperty.name != "m-Script");
                    prop.Bind(serializeObj);
                    infoBox.Add(prop);

                    // Update prefab
                    if (dataProperty.name == "Prefab")
                    {
                        prop.RegisterCallback<ChangeEvent<UnityEngine.Object>>((changeEvt) => LoadLevelPrefab(data));
                    }
                }

                LoadLevelPrefab(data);
            }
        };

        list.Rebuild();

        if (!isInitialized) 
            list.SetSelection(listIndex);
    }

    public override void SetButtons()
    {
        base.SetButtons();

        Button refreshButton = rootVisualElement.Query<Button>("refresh").First();
        refreshButton.clicked += RefreshScripts; 
    }

    public override void AddButton()
    {
        window = CreateInstance<LevelPopupWindow>();
        window.addButtonPressed = true;
        isPopupActive = true;
        window.window = this;

        Vector2 mousePos = GUIUtility.GUIToScreenPoint(UnityEngine.Event.current.mousePosition);
        window.position = new Rect(mousePos.x, mousePos.y, 300, 400);
        window.ShowPopup();
    }

    public override void DeleteButton()
    {
        if (list.selectedItem != null)
        {
            LevelData selectedLevel = list.selectedItem as LevelData;
            if (!EditorUtility.DisplayDialog($"Delete Level", $"Delete {selectedLevel.name}?", "Delete", "Cancel"))
                return;

            list.ClearSelection();
            rootVisualElement.Query<Box>("level-info").First().Clear();
            list.itemsSource = null;

            AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(selectedLevel.Guid));

            CreateListView();

            Box gameObjectPreview = rootVisualElement.Query<Box>("object-preview").First();
            gameObjectPreview.Clear();
        }
    }

    public override void RenameButton()
    {
        if (list.selectedItem != null)
        {
            window = CreateInstance<LevelPopupWindow>();
            window.renameButtonPressed = true;
            isPopupActive = true;
            window.window = this;

            Vector2 mousePos = GUIUtility.GUIToScreenPoint(UnityEngine.Event.current.mousePosition);
            window.position = new Rect(mousePos.x, mousePos.y, 300, 100);
            window.ShowPopup();
        }
    }

    private void RefreshScripts()
    {
        EditorUtility.RequestScriptReload();
    }

    private void FindAllLevels(out List<LevelData> levels)
    {
        string[] guids = AssetDatabase.FindAssets("t:LevelData");

        levels = new List<LevelData>();

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);

            LevelData loadedData = AssetDatabase.LoadAssetAtPath<LevelData>(path);
            loadedData.Guid = guids[i];

            levels.Add(loadedData);
        }
    }

    private void LoadLevelPrefab(LevelData levelData)
    {
        if (levelData.Prefab == null)
            return;

        Box gameObjectPreview = rootVisualElement.Query<Box>("object-preview").First();
        gameObjectPreview.Clear();

        GUIStyle bgColor = new GUIStyle();
        bgColor.normal.background = EditorGUIUtility.whiteTexture;

        if (isInitialized)
            DestroyImmediate(gameObjectEditor);

        gameObjectEditor = Editor.CreateEditor(levelData.Prefab);
        IMGUIContainer container = new IMGUIContainer(() => { gameObjectEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(1000, 500), bgColor); });
        gameObjectPreview.Add(container);
    }

}
