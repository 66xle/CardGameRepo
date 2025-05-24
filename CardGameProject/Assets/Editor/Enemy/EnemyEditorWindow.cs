using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyEditorWindow : BaseEditorWindow
{
    GameObject gameObject;
    Editor gameObjectEditor;

    [MenuItem("Editor/Enemy Editor")]
    public static void ShowWindow()
    {
        EnemyEditorWindow window = GetWindow<EnemyEditorWindow>();
        ShowWindow(window, "Enemy Editor");
    }

    [InitializeOnLoadMethod]
    private static void OnLoad()
    {
        listIndex = SessionState.GetInt("enemyListIndex", 0);
        isInitialized = false;
        editorReadyToInit = true;
    }

    public override void Init()
    {
        Enable("EnemyEditorWindow", "EnemyEditorStyles", "enemy", "Enemy");

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
        FindAllEnemies(out List<EnemyData> enemies);

        DropdownField dropdownField = rootVisualElement.Query<DropdownField>("filter");
        
        if (dropdownField.value != "Any")
        {
            enemies = enemies.Where(data => data.EnemyType.ToString() == dropdownField.value).ToList();
        }

        List<string> pathList = enemies.Select(data => AssetDatabase.GUIDToAssetPath(data.Guid)).ToList();
        SetupListView(enemies, pathList, "enemy-list");

        list.selectionChanged += (enumerable) =>
        {
            if (isInitialized)
                SessionState.SetInt("enemyListIndex", list.selectedIndex); 

            foreach (UnityEngine.Object it in enumerable)
            {
                Box enemyDataInfoBox = rootVisualElement.Query<Box>("enemy-info").First();
                enemyDataInfoBox.Clear();

                Box gameObjectPreview = rootVisualElement.Query<Box>("object-preview").First();
                gameObjectPreview.Clear();

                EnemyData enemyData = it as EnemyData;

                if (enemyData == null) return;

                SerializedObject serializeEnemy = new SerializedObject(enemyData);
                SerializedProperty enemyDataProperty = serializeEnemy.GetIterator();
                enemyDataProperty.Next(true);

                while (enemyDataProperty.NextVisible(false))
                {
                    PropertyField prop = new PropertyField(enemyDataProperty);

                    prop.SetEnabled(enemyDataProperty.name != "m-Script");
                    prop.Bind(serializeEnemy);
                    enemyDataInfoBox.Add(prop);

                    // Update prefab
                    if (enemyDataProperty.name == "Prefab")
                    {
                        prop.RegisterCallback<ChangeEvent<UnityEngine.Object>>((changeEvt) => LoadEnemyPrefab(enemyData));
                    }
                }

                LoadEnemyPrefab(enemyData);
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
        window = CreateInstance<EnemyPopupWindow>();
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
            EnemyData selectedEnemy = list.selectedItem as EnemyData;
            if (!EditorUtility.DisplayDialog($"Delete Enemy", $"Delete {selectedEnemy.name}?", "Delete", "Cancel"))
                return;

            list.ClearSelection();
            rootVisualElement.Query<Box>("enemy-info").First().Clear();
            list.itemsSource = null;

            AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(selectedEnemy.Guid));

            CreateListView();

            Box gameObjectPreview = rootVisualElement.Query<Box>("object-preview").First();
            gameObjectPreview.Clear();
        }
    }

    public override void RenameButton()
    {
        if (list.selectedItem != null)
        {
            window = CreateInstance<EnemyPopupWindow>();
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

    private void FindAllEnemies(out List<EnemyData> enemies)
    {
        string[] guids = AssetDatabase.FindAssets("t:EnemyData");

        enemies = new List<EnemyData>();

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);

            EnemyData loadedEnemyData = AssetDatabase.LoadAssetAtPath<EnemyData>(path);
            loadedEnemyData.Guid = guids[i];

            enemies.Add(loadedEnemyData);
        }
    }

    private void LoadEnemyPrefab(EnemyData enemyData)
    {
        if (enemyData.Prefab == null)
            return;

        Box gameObjectPreview = rootVisualElement.Query<Box>("object-preview").First();
        gameObjectPreview.Clear();

        GUIStyle bgColor = new GUIStyle();
        bgColor.normal.background = EditorGUIUtility.whiteTexture;

        if (isInitialized)
            DestroyImmediate(gameObjectEditor);

        gameObjectEditor = Editor.CreateEditor(enemyData.Prefab);
        IMGUIContainer container = new IMGUIContainer(() => { gameObjectEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(1000, 500), bgColor); });
        gameObjectPreview.Add(container);
    }

}
