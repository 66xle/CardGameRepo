using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;


#if UNITY_EDITOR
using UnityEditor;
#endif

[UxmlElement]
public partial class DynamicDropdown : VisualElement
{
    // ================= UI =================
    private Button _toggleButton;
    private Label _label;
    private Label _arrow;

    private VisualElement _dropdownContent;
    private VisualElement _itemList;

    private TextField _inputField;
    private Button _addButton;

    // ================= DATA =================
    private List<string> _items = new();
    private string _selectedItem;

    // ================= STATE =================
    private bool _isPlaceholderActive = true;
    private const string ADDTEXT = "Add Item";
    private const string DEFAULTTEXT = "Base Card";

    // ================= EVENTS =================
    public event Action<string> OnItemSelected;
    public event Action<string> OnItemAdded;

    public DynamicDropdown()
    {
        style.flexDirection = FlexDirection.Column;

        BuildToggle();
        BuildDropdown();

        AddDefaultItem("Default");

        Add(_toggleButton);
        Add(_dropdownContent);
    }

    // =========================================================
    // TOGGLE
    // =========================================================
    private void BuildToggle()
    {
        _toggleButton = new Button(ToggleDropdown);
        _toggleButton.style.flexDirection = FlexDirection.Row;
        _toggleButton.style.justifyContent = Justify.SpaceBetween;
        _toggleButton.style.alignItems = Align.Center;

        _label = new Label(DEFAULTTEXT);
        _label.style.flexGrow = 1;

        _arrow = new Label("▼");
        _arrow.style.width = 20;
        _arrow.style.unityTextAlign = TextAnchor.MiddleCenter;

        _toggleButton.Add(_label);
        _toggleButton.Add(_arrow);
    }

    // =========================================================
    // DROPDOWN
    // =========================================================
    private void BuildDropdown()
    {
        _dropdownContent = new VisualElement();
        _dropdownContent.style.display = DisplayStyle.None;

        _dropdownContent.style.backgroundColor = new Color(0.15f, 0.15f, 0.15f);
        _dropdownContent.style.borderTopWidth = 1;
        _dropdownContent.style.borderBottomWidth = 1;
        _dropdownContent.style.borderLeftWidth = 1;
        _dropdownContent.style.borderRightWidth = 1;

        _itemList = new VisualElement();

        _dropdownContent.Add(_itemList);
        _dropdownContent.Add(BuildDivider());
        _dropdownContent.Add(BuildInputContainer());
    }

    // =========================================================
    // INPUT FIELD + ADD BUTTON
    // =========================================================
    private VisualElement BuildInputContainer()
    {
        var container = new VisualElement();
        container.style.flexDirection = FlexDirection.Row;
        container.style.alignItems = Align.Center;

        _inputField = new TextField();
        _inputField.value = ADDTEXT;
        _inputField.style.flexGrow = 1;

        _inputField.RegisterCallback<FocusInEvent>(_ =>
        {
            if (_isPlaceholderActive)
            {
                _inputField.value = "";
                _isPlaceholderActive = false;
            }
        });

        _inputField.RegisterCallback<FocusOutEvent>(_ =>
        {
            if (string.IsNullOrWhiteSpace(_inputField.value))
            {
                _inputField.value = ADDTEXT;
                _isPlaceholderActive = true;
            }

            ValidateInput();
        });

        _inputField.RegisterValueChangedCallback(_ => ValidateInput());

        _addButton = new Button(() =>
        {
            AddItem(_inputField.value);
        })
        {
            text = "+"
        };

        _addButton.style.width = 24;
        _addButton.SetEnabled(false);

        container.Add(_inputField);
        container.Add(_addButton);

        return container;
    }

    // =========================================================
    // DIVIDER
    // =========================================================
    private VisualElement BuildDivider()
    {
        var line = new VisualElement();
        line.style.height = 1;
        line.style.backgroundColor = new Color(0.3f, 0.3f, 0.3f);
        return line;
    }

    // =========================================================
    // OPEN / CLOSE
    // =========================================================
    private void ToggleDropdown()
    {
        bool opening = _dropdownContent.style.display == DisplayStyle.None;

        _dropdownContent.style.display = opening
            ? DisplayStyle.Flex
            : DisplayStyle.None;

        _arrow.text = opening ? "▲" : "▼";
    }

    private void CloseDropdown()
    {
        _dropdownContent.style.display = DisplayStyle.None;
        _arrow.text = "▼";
    }

    // =========================================================
    // DEFAULT ITEM
    // =========================================================
    private void AddDefaultItem(string name)
    {
        _selectedItem = name;
        _label.text = name;

        _items.Add(name);

        CreateItem(name, isDefault: true);
    }

    // =========================================================
    // ADD ITEM
    // =========================================================
    private void AddItem(string name)
    {
        if (_isPlaceholderActive) return;
        if (string.IsNullOrWhiteSpace(name)) return;
        if (_items.Contains(name)) return;

        _items.Add(name);
        CreateItem(name);

        _inputField.value = DEFAULTTEXT;
        _isPlaceholderActive = true;

        ValidateInput();

        OnItemAdded?.Invoke(name);
    }

    public void AddItemExternal(string name)
    {
        if (_items.Contains(name)) return;
        _items.Add(name);
        CreateItem(name);
    }

    public void ClearItems()
    {
        _items.Clear();
        _itemList.Clear();
        AddDefaultItem(DEFAULTTEXT);
    }

    // =========================================================
    // VALIDATION
    // =========================================================
    private void ValidateInput()
    {
        string value = _inputField.value;

        bool valid =
            !_isPlaceholderActive &&
            !string.IsNullOrWhiteSpace(value) &&
            !_items.Contains(value);

        _addButton.SetEnabled(valid);
    }

    // =========================================================
    // ITEM CREATION
    // =========================================================
    private void CreateItem(string name, bool isDefault = false)
    {
        var row = new VisualElement();

        row.style.flexDirection = FlexDirection.Row;
        row.style.alignItems = Align.Center;
        row.style.paddingLeft = 4;
        row.style.paddingRight = 4;
        row.style.paddingTop = 2;
        row.style.paddingBottom = 2;

        var label = new Label(name);
        label.style.flexGrow = 1;

        // hover highlight
        row.RegisterCallback<MouseEnterEvent>(_ =>
        {
            if (_selectedItem != name)
                row.style.backgroundColor = new Color(0.25f, 0.25f, 0.25f);
        });

        row.RegisterCallback<MouseLeaveEvent>(_ =>
        {
            if (_selectedItem != name)
                row.style.backgroundColor = StyleKeyword.None;
        });

        // select
        row.RegisterCallback<ClickEvent>(_ =>
        {
            SelectItem(name);
            RefreshHighlights();
            CloseDropdown();
        });

        row.Add(label);

        // delete with confirmation (Editor only)
        if (!isDefault)
        {
            var remove = new Button(() =>
            {
                ConfirmDelete(name, row);
            })
            {
                text = "X"
            };

            remove.style.width = 20;
            row.Add(remove);
        }

        _itemList.Add(row);

        RefreshHighlights();
    }

    // =========================================================
    // DELETE CONFIRMATION (EDITOR ONLY)
    // =========================================================
    private void ConfirmDelete(string name, VisualElement row)
    {
#if UNITY_EDITOR
        bool result = EditorUtility.DisplayDialog(
            "Delete Item",
            $"Are you sure you want to delete '{name}'?",
            "Delete",
            "Cancel"
        );

        if (!result) return;
#endif

        DeleteItem(name, row);
    }

    private void DeleteItem(string name, VisualElement row)
    {
        if (_selectedItem == name)
        {
            _selectedItem = DEFAULTTEXT;
            _label.text = _selectedItem;
        }

        _items.Remove(name);
        row.RemoveFromHierarchy();

        RefreshHighlights();
        ValidateInput();
    }

    // =========================================================
    // SELECTION
    // =========================================================
    private void SelectItem(string name)
    {
        _selectedItem = name;
        _label.text = name;

        OnItemSelected?.Invoke(name);
    }

    public string GetSelectedItem()
    {
        return _selectedItem;
    }

    // =========================================================
    // HIGHLIGHTING
    // =========================================================
    private void RefreshHighlights()
    {
        foreach (var child in _itemList.Children())
        {
            var label = child.Q<Label>();
            if (label == null) continue;

            bool selected = label.text == _selectedItem;

            child.style.backgroundColor = selected
                ? new Color(0.3f, 0.3f, 0.3f)
                : StyleKeyword.None;
        }
    }
}
