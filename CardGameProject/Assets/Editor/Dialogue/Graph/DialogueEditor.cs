using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;

public class DialogueEditor : EditorWindow
{
    private DialogueGraphView _graphView;
    private InspectorView inspectorView;
    private string _fileName = "New Dialogue";

    private TextField fileNameTextField;

    public ListView eventList;

    [MenuItem("Editor/Dialogue Graph")]
    public static void ShowWindow()
    {
        DialogueEditor window = GetWindow<DialogueEditor>("Graph");
        window.titleContent = new GUIContent("Dialogue System");
    }

    private void OnEnable()
    {
        ConstructGraphView();
    }

    private void OnDisable()
    {
        //rootVisualElement.Remove(_graphView);
    }

    private void ConstructGraphView()
    {
        _graphView = new DialogueGraphView()
        {
            name = "Dialogue Graph"
        };

        GenerateToolBar();

        // Load uxml
        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/Dialogue/Graph/Resources/DialogueGraph.uxml");
        visualTree.CloneTree(rootVisualElement);
        rootVisualElement.Add(_graphView);
        
        inspectorView = rootVisualElement.Q<InspectorView>();
        _graphView = rootVisualElement.Q<DialogueGraphView>();
        _graphView.AddSearchWindow(this);
        _graphView.OnNodeSelected = OnNodeSelectionChanged;


        CreateEventListView();
    }

    void OnNodeSelectionChanged(DialogueNode node)
    {
        inspectorView.UpdateSelection(node);
    }

    private void GenerateToolBar()
    {
        Toolbar toolbar = new Toolbar();

        fileNameTextField = new TextField("File Name");
        fileNameTextField.SetValueWithoutNotify(_fileName);
        fileNameTextField.MarkDirtyRepaint();
        fileNameTextField.RegisterValueChangedCallback(evt => _fileName = evt.newValue);
        toolbar.Add(fileNameTextField);

        //toolbar.Add(new Button(() => RequestDataOperation(true)) { text = "Save Data" });
        //toolbar.Add(new Button(() => RequestDataOperation(false)) { text = "Load Data" });

        rootVisualElement.Add(toolbar);
    }

    private void CreateEventListView()
    {
        FindAllEvents(out List<Event> events);

        eventList = rootVisualElement.Query<ListView>("event-list").First();
        eventList.makeItem = () => new Label();
        eventList.bindItem = (element, i) => (element as Label).text = Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(events[i].guid));

        eventList.itemsSource = events;
        eventList.itemHeight = 16;
        eventList.selectionType = SelectionType.Single;

        eventList.onSelectionChange += (enumerable) =>
        {
            foreach (UnityEngine.Object it in enumerable)
            {
                Event selectedEvent = it as Event;

                // Do check save work

                RequestDataOperation(false, selectedEvent.guid);

                //Box cardInfoBox = rootVisualElement.Query<Box>("card-info").First();
                //cardInfoBox.Clear();

                //Card card = it as Card;

                //SerializedObject serializeCard = new SerializedObject(card);
                //SerializedProperty cardProperty = serializeCard.GetIterator();
                //cardProperty.Next(true);

                //while (cardProperty.NextVisible(false))
                //{
                //    PropertyField prop = new PropertyField(cardProperty);

                //    prop.SetEnabled(cardProperty.name != "m-Script");
                //    prop.Bind(serializeCard);
                //    cardInfoBox.Add(prop);


                //}

            }
        };

        eventList.Refresh();
    }

    private void FindAllEvents(out List<Event> events)
    {
        string[] guids = AssetDatabase.FindAssets("t:Event");

        events = new List<Event>();

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);

            Event loadedEvent = AssetDatabase.LoadAssetAtPath<Event>(path);
            loadedEvent.guid = guids[i];

            events.Add(loadedEvent);
        }
    }

    private void RequestDataOperation(bool save, string guid)
    {
        GraphSaveUtility saveUtility = GraphSaveUtility.GetInstance(_graphView);
        if (save)
        {
            if (string.IsNullOrEmpty(_fileName))
            {
                EditorUtility.DisplayDialog("Invalid file name!", "Please enter a valid file name.", "OK");
                return;
            }

            saveUtility.SaveGraph(_fileName);
        }
        else
        {
            saveUtility.LoadGraph(fileNameTextField, guid);
        }
    }
}
