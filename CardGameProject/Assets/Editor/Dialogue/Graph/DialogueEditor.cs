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

    public ListView eventList;
    private PopupWindow window;
    public bool isPopupActive;

    private int selectedIndex;

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
    private void OnFocus()
    {
        if (isPopupActive)
        {
            //window.Focus();
            //EditorUtility.DisplayDialog($"Error", $"Currently creating card", "Ok");
        }
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

        toolbar.Add(new Button(() => AddEvent()) { text = "Add Event" });
        toolbar.Add(new Button(() => DeleteEvent()) { text = "Delete Event" });
        toolbar.Add(new Button(() => RenameEvent()) { text = "Rename Event" });
        toolbar.Add(new Button(() => RequestDataOperation(true)) { text = "Save Event" });

        rootVisualElement.Add(toolbar);
    }

    private void AddEvent()
    {
        window = CreateInstance<PopupWindow>();
        window.addEventButtonPressed = true;
        isPopupActive = true;
        window.dialogueWindow = this;

        Vector2 mousePos = GUIUtility.GUIToScreenPoint(UnityEngine.Event.current.mousePosition);
        window.position = new Rect(mousePos.x, mousePos.y, 200, 100);
        window.ShowPopup();
    }

    private void DeleteEvent()
    {
        if (eventList.selectedItem != null)
        {
            Event selectedEvent = eventList.selectedItem as Event;
            if (!EditorUtility.DisplayDialog($"Delete Event", $"Delete {selectedEvent.name}?", "Delete", "Cancel"))
                return;

            eventList.ClearSelection();
            ClearGraph();
            eventList.itemsSource = null;

            AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(selectedEvent.guid));

            
            CreateEventListView();
        }
    }

    public void ClearGraph()
    {
        GraphSaveUtility.GetInstance(_graphView).ClearGraph();
    }
    private void RenameEvent()
    {
        window = CreateInstance<PopupWindow>();
        window.renameEventButtonPressed = true;
        isPopupActive = true;
        window.dialogueWindow = this;

        Vector2 mousePos = GUIUtility.GUIToScreenPoint(UnityEngine.Event.current.mousePosition);
        window.position = new Rect(mousePos.x, mousePos.y, 200, 100);
        window.ShowPopup();
    }

    public void CreateEventListView()
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

                // EVENT LIST IS RUNNING MULITPLE TIMES WHEN THIS FUNCTION IS RAN AGAIN - NEED TO FIX
                Debug.Log("test");

                return;

                if (eventList.selectedIndex >= 0)
                {
                    Event eventContainer = GraphSaveUtility.GetInstance(_graphView).GetEventData();

                    if (selectedEvent != eventContainer)
                    {
                        Debug.Log("not the same");
                    }
                    else
                    {
                        Debug.Log("is the same");
                    }

                    //if (selectedIndex != eventList.selectedIndex)
                    //{
                    //    eventList.SetSelection(selectedIndex);
                    //    return;
                    //}
                }

                RequestDataOperation(false);
                selectedIndex = eventList.selectedIndex;
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

    private void RequestDataOperation(bool save)
    {
        // Check if user has selected an event
        if (eventList.selectedItem == null)
        {
            EditorUtility.DisplayDialog($"Error", $"No Event Selected!", "Ok");
            return;
        }

        Event selectedEvent = eventList.selectedItem as Event;
        

        GraphSaveUtility saveUtility = GraphSaveUtility.GetInstance(_graphView);
        if (save)
        {
            string fileName = Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(selectedEvent.guid));

            saveUtility.SaveGraph(fileName);
        }
        else
        {
            saveUtility.LoadGraph(selectedEvent.guid);
        }
    }
}
