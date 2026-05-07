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
    private VisualElement _inputText;
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
    public event Action<string> OnItemDeleted;
    public event Action<string, string> OnItemRenamed;

    public DynamicDropdown()
    {
        style.flexDirection = FlexDirection.Column;

        BuildToggle();
        BuildDropdown();

        AddDefaultItem(DEFAULTTEXT);

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

        _inputText = _inputField.Query(className: "unity-text-element");
        _inputText.style.color = Color.gray;

        _inputField.RegisterCallback<FocusInEvent>(_ =>
        {
            if (_isPlaceholderActive)
            {
                _inputField.value = "";
                _isPlaceholderActive = false;
                _inputText.style.color = Color.white;
            }
        });

        _inputField.RegisterCallback<FocusOutEvent>(_ =>
        {
            if (string.IsNullOrWhiteSpace(_inputField.value))
            {
                _inputField.value = ADDTEXT;
                _isPlaceholderActive = true;
                _inputText.style.color = Color.gray;
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

    // =========================================================
    // DEFAULT ITEM
    // =========================================================
    private void AddDefaultItem(string name)
    {
        _selectedItem = name;
        _label.text = name;

        _items.Add(name);

        CreateItem(name, true);
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

        _inputField.value = ADDTEXT;
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

        if (valid)
            _inputText.style.color = Color.white;
        else
            _inputText.style.color = Color.red;

        if (_isPlaceholderActive)
            _inputText.style.color = Color.gray;

        _addButton.SetEnabled(valid);
        _addButton.style.opacity = valid ? 1f : 0.5f;
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

        string currentName = name;

        var label = new Label(name);
        label.style.flexGrow = 1;

        var renameField = new TextField();
        renameField.style.flexGrow = 1;
        renameField.style.display = DisplayStyle.None;

        row.RegisterCallback<MouseEnterEvent>(_ =>
        {
            if (_selectedItem != currentName)
                row.style.backgroundColor = new Color(0.25f, 0.25f, 0.25f);
        });

        row.RegisterCallback<MouseLeaveEvent>(_ =>
        {
            if (_selectedItem != currentName)
                row.style.backgroundColor = StyleKeyword.None;
        });

        row.RegisterCallback<ClickEvent>(_ =>
        {
            if (renameField.style.display == DisplayStyle.Flex)
                return;

            SelectItem(currentName);

            RefreshHighlights();
        });

        row.Add(label);
        row.Add(renameField);

        if (!isDefault)
        {
            bool renaming = false;

            var renameButton = new Button()
            {
                text = "R"
            };

            renameButton.style.width = 20;

            var deleteButton = new Button()
            {
                text = "X"
            };

            deleteButton.style.width = 20;

            bool IsValidRename(string value)
            {
                return
                    !string.IsNullOrWhiteSpace(value) &&
                    value != currentName &&
                    !_items.Contains(value);
            }

            void UpdateRenameValidation()
            {
                bool valid = IsValidRename(renameField.value);

                renameField.Q(TextInputBaseField<string>.textInputUssName)
                    .style.color = valid
                        ? Color.white
                        : Color.red;

                deleteButton.SetEnabled(valid);
                deleteButton.style.opacity = valid ? 1f : 0.5f;
            }

            void ExitRenameMode()
            {
                renaming = false;

                label.style.display = DisplayStyle.Flex;
                renameField.style.display = DisplayStyle.None;

                renameButton.text = "R";
                deleteButton.text = "X";

                deleteButton.SetEnabled(true);
                deleteButton.style.opacity = 1f;

                renameField.value = currentName;
            }

            renameButton.clicked += () =>
            {
                if (!renaming)
                {
                    renaming = true;

                    renameField.value = currentName;

                    label.style.display = DisplayStyle.None;
                    renameField.style.display = DisplayStyle.Flex;

                    renameButton.text = "C";
                    deleteButton.text = "✔";

                    renameField.Focus();

                    UpdateRenameValidation();

                    return;
                }

                ExitRenameMode();
            };

            deleteButton.clicked += () =>
            {
                if (renaming)
                {
                    string newName = renameField.value;

                    if (!IsValidRename(newName))
                        return;

                    int index = _items.IndexOf(currentName);

                    if (index >= 0)
                        _items[index] = newName;

                    if (_selectedItem == currentName)
                    {
                        _selectedItem = newName;
                        _label.text = newName;
                    }

                    OnItemRenamed?.Invoke(currentName, newName);

                    currentName = newName;
                    label.text = newName;

                    ExitRenameMode();

                    RefreshHighlights();

                    return;
                }

                ConfirmDelete(currentName, row);

                if (_selectedItem == currentName)
                {
                    SelectItem(DEFAULTTEXT);
                }
            };

            renameField.RegisterValueChangedCallback(_ =>
            {
                UpdateRenameValidation();
            });

            row.Add(renameButton);
            row.Add(deleteButton);
        }

        _itemList.Add(row);

        RefreshHighlights();
    }

    // =========================================================
    // DELETE CONFIRMATION
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

        OnItemDeleted?.Invoke(name);

        SelectItem(DEFAULTTEXT);
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

            if (label == null)
                continue;

            bool selected = label.text == _selectedItem;

            child.style.backgroundColor = selected
                ? new Color(0.3f, 0.3f, 0.3f)
                : StyleKeyword.None;
        }
    }
}
