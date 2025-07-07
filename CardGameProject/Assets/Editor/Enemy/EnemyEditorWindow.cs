using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Codice.CM.Interfaces;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyEditorWindow : BaseEditorWindow
{
    GameObject gameObject;
    Editor gameObjectEditor;

    CustomPreviewHandler previewHandler;
    IMGUIContainer container;

    List<AnimationClipData> clipDataList;
    bool IsPlayingAnimation;

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

    private void OnDisable()
    {
        previewHandler?.Cleanup();
        EditorApplication.update -= RepaintPreview;
    }

    public override void Init()
    {
        Enable("EnemyEditorWindow", "EnemyEditorStyles", "enemy", "Enemy");

        EditorApplication.delayCall += () => 
        {
            base.Init();

            IsPlayingAnimation = false;

            DropdownField dropdownField = rootVisualElement.Query<DropdownField>("filter");
            dropdownField.RegisterCallback<ChangeEvent<string>>((evt) =>
            {
                dropdownField.value = evt.newValue;
                CreateListView();
            });

            EditorApplication.update += RepaintPreview;
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

                    if (enemyDataProperty.name == "WeaponType")
                    {
                        prop.RegisterValueChangeCallback(changeEvt => GetAnimationList(enemyData));
                    }

                }

                LoadEnemyPrefab(enemyData);
                GetAnimationList(enemyData);
            }
        };

        list.Rebuild();

        if (!isInitialized) 
            list.SetSelection(listIndex);
    }

    #region Buttons

    public override void SetButtons()
    {
        base.SetButtons();

        Button refreshButton = rootVisualElement.Query<Button>("refresh").First();
        refreshButton.clicked += RefreshScripts;

        Button playButton = rootVisualElement.Query<Button>("play").First();
        playButton.clicked += PlayButton;
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

    public void PlayButton()
    {
        if (previewHandler.IsClipEmpty()) return;

        Button playButton = rootVisualElement.Query<Button>("play").First();

        if (IsPlayingAnimation)
        {
            playButton.text = "Play";
            previewHandler.PauseClip();
            IsPlayingAnimation = false;
            return;
        }

        Pause();
    }

    void Pause()
    {
        Button playButton = rootVisualElement.Query<Button>("play").First();
        playButton.text = "Pause";
        previewHandler.PlayClip();
        IsPlayingAnimation = true;
    }

    #endregion

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

    private void GetAnimationList(EnemyData enemyData)
    {
        List<AnimationClipData> animationClipDataList = new();
        enemyData.WeaponTypeAnimationSet.ForEach(data => animationClipDataList.AddRange(data.AnimationClipDataList));

        clipDataList = animationClipDataList;

        DropdownField animationField = rootVisualElement.Query<DropdownField>("animationField");
        animationField.choices.Clear();
        animationField.choices.Add("None");
        animationField.index = 0;
        animationClipDataList.ForEach(clipData => animationField.choices.Add(clipData.Clip.name));

        EventCallback<ChangeEvent<string>> animationCallback = null;

        animationCallback = evt =>
        {
            if (IsPlayingAnimation)
                Pause();

            string clipName = animationField.value;

            if (clipName == "None")
            {
                previewHandler.SetClipData(null);
                return;
            }

            AnimationClipData clipData = animationClipDataList.First(clipData => clipData.Clip.name == clipName);
            previewHandler.SetClipData(clipData);
        };

        animationField.UnregisterValueChangedCallback(animationCallback);
        animationField.RegisterValueChangedCallback(animationCallback);
    }

    private void LoadEnemyPrefab(EnemyData enemyData)
    {
        if (enemyData.Prefab == null)
            return;

        Box gameObjectPreview = rootVisualElement.Query<Box>("object-preview").First();
        gameObjectPreview.Clear();

        if (isInitialized)
            DestroyImmediate(gameObjectEditor);

        gameObjectEditor = Editor.CreateEditor(enemyData.Prefab);
        //gameObjectEditor.OnInteractivePreviewGUI(rect, GUIStyle.none);

        if (previewHandler == null)
            previewHandler = new CustomPreviewHandler();

        previewHandler.Init(enemyData.Prefab);

        container = new IMGUIContainer(() =>
        {
            Rect r = GUILayoutUtility.GetRect(800, 600);
            previewHandler.OnPreviewGUI(r);
        });

        container.style.flexGrow = 1;
        gameObjectPreview.Add(container);
    }

    void RepaintPreview()
    {
        if (container != null && IsPlayingAnimation)
            container.MarkDirtyRepaint();
    }

    
}
