using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class EncounterEditorWindow : BaseEditorWindow
{
    GameObject gameObject;
    List<Editor> editors;

    [MenuItem("Editor/Encounter Editor")]
    public static void ShowWindow()
    {
        EncounterEditorWindow window = GetWindow<EncounterEditorWindow>();
        ShowWindow(window, "Encounter Editor");
    }

    [InitializeOnLoadMethod]
    private static void OnLoad()
    {
        listIndex = SessionState.GetInt("encounterListIndex", 0);
        isInitialized = false;
        editorReadyToInit = true;
    }

    public override void Init()
    {
        Enable("EncounterEditorWindow", "EncounterEditorStyles", "encounter", "Encounter");

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
        FindAllEncounters(out List<EncounterData> encounters);

        
        //DropdownField dropdownField = rootVisualElement.Query<DropdownField>("filter");
        
        //if (dropdownField.value != "Any")
        //{
        //    levels = levels.Where(data => data.IsFixed == true).ToList();
        //}

        List<string> pathList = encounters.Select(data => AssetDatabase.GUIDToAssetPath(data.Guid)).ToList();
        SetupListView(encounters, pathList, "encounter-list");

        list.selectionChanged += (enumerable) =>
        {
            if (isInitialized)
                SessionState.SetInt("encounterListIndex", list.selectedIndex); 

            foreach (UnityEngine.Object it in enumerable)
            {

                Box infoBox = rootVisualElement.Query<Box>("encounter-info").First();
                infoBox.Clear();

                Box preview = rootVisualElement.Query<Box>("object-preview").First();
                preview.Clear();

                EncounterData data = it as EncounterData;

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
                    //if (dataProperty.name == "Enemies")
                    //{
                    //    prop.RegisterCallback<ChangeEvent<UnityEngine.Object>>((changeEvt) => LoadEnemies(data));
                    //}
                }

                //LoadEnemies(data);
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
        window = CreateInstance<EncounterPopupWindow>();
        window.addButtonPressed = true;
        isPopupActive = true;
        window.window = this;

        Vector2 mousePos = GUIUtility.GUIToScreenPoint(UnityEngine.Event.current.mousePosition);
        window.position = new Rect(mousePos.x, mousePos.y, 300, 200);
        window.ShowPopup();
    }

    public override void DeleteButton()
    {
        if (list.selectedItem != null)
        {
            EncounterData selected = list.selectedItem as EncounterData;
            if (!EditorUtility.DisplayDialog($"Delete Encounter", $"Delete {selected.name}?", "Delete", "Cancel"))
                return;

            list.ClearSelection();
            rootVisualElement.Query<Box>("encounter-info").First().Clear();
            list.itemsSource = null;

            AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(selected.Guid));

            CreateListView();

            Box gameObjectPreview = rootVisualElement.Query<Box>("object-preview").First();
            gameObjectPreview.Clear();
        }
    }

    public override void RenameButton()
    {
        if (list.selectedItem != null)
        {
            window = CreateInstance<EncounterPopupWindow>();
            window.renameButtonPressed = true;
            isPopupActive = true;
            window.window = this;

            Vector2 mousePos = GUIUtility.GUIToScreenPoint(UnityEngine.Event.current.mousePosition);
            window.position = new Rect(mousePos.x, mousePos.y, 300, 200);
            window.ShowPopup();
        }
    }

    private void RefreshScripts()
    {
        EditorUtility.RequestScriptReload();
    }

    private void FindAllEncounters(out List<EncounterData> encounters)
    {
        string[] guids = AssetDatabase.FindAssets("t:EncounterData");

        encounters = new List<EncounterData>();

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);

            EncounterData loadedData = AssetDatabase.LoadAssetAtPath<EncounterData>(path);
            loadedData.Guid = guids[i];

            encounters.Add(loadedData);
        }
    }

    //private void LoadEnemies(EncounterData encounterData)
    //{
    //    if (isInitialized)
    //        editors.ForEach(e => DestroyImmediate(e));

    //    if (encounterData.Enemies.Count == 0)
    //        return;

    //    Box gameObjectPreview = rootVisualElement.Query<Box>("object-preview").First();
    //    gameObjectPreview.Clear();

    //    GUIStyle bgColor = new GUIStyle();
    //    bgColor.normal.background = EditorGUIUtility.whiteTexture;

    //    List<Object> prefabs = new();

    //    foreach (EnemyData data in encounterData.Enemies)
    //    {
    //        if (data == null)
    //            continue;

    //        if (data.Prefab != null)
    //            prefabs.Add(data.Prefab);
    //    }

    //    if (prefabs.Count == 0) return;

    //    for (int i = 0; i < prefabs.Count; i++)
    //    {
    //        Editor editor = Editor.CreateEditor(prefabs[i]);
    //        editors.Add(editor);

    //        IMGUIContainer container = new IMGUIContainer(() => { editor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(1000, 500), bgColor); });

    //        gameObjectPreview.Add(container);
    //    }
    //}

}
