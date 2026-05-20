using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class LabelCheckboxField : VisualElement
{
    public enum FieldMode
    {
        Float,
        Object
    }

    // Elements
    public Toggle Checkbox { get; private set; }

    public FloatField ValueField { get; private set; }
    public ObjectField ObjectField { get; private set; }

    private readonly Label _titleLabel;
    private readonly Label _subtitleLabel;
    private readonly Label _unitLabel;

    private readonly VisualElement _valueContainer;

    // Properties
    [UxmlAttribute]
    public string Title
    {
        get => _titleLabel.text;
        set => _titleLabel.text = value;
    }

    public string Subtitle
    {
        get => _subtitleLabel.text;
        set => _subtitleLabel.text = value;
    }

    [UxmlAttribute]
    public string Unit
    {
        get => _unitLabel.text;
        set => _unitLabel.text = value;
    }

    public bool Checked
    {
        get => Checkbox.value;
        set => Checkbox.value = value;
    }

    private FieldMode _mode = FieldMode.Float;

    [UxmlAttribute]
    public FieldMode Mode
    {
        get => _mode;
        set
        {
            _mode = value;
            RefreshFieldMode();
        }
    }

    // Float Value
    public float FloatValue
    {
        get => ValueField.value;
        set => ValueField.value = value;
    }

    // Object Value
    public Object ObjectValue
    {
        get => ObjectField.value;
        set => ObjectField.value = value;
    }

    public LabelCheckboxField()
    {
        // ROOT
        style.flexDirection = FlexDirection.Row;
        style.justifyContent = Justify.SpaceBetween;
        style.alignItems = Align.Center;

        style.paddingRight = 12;
        style.paddingTop = 8;
        style.paddingBottom = 8;

        style.height = 56;

        style.backgroundColor = new StyleColor(new Color32(51, 51, 51, 255));

        // RIGHT SIDE
        var rightContainer = new VisualElement();
        rightContainer.style.flexDirection = FlexDirection.Row;
        rightContainer.style.alignItems = Align.Center;

        // CHECKBOX
        Checkbox = new Toggle();
        Checkbox.text = string.Empty;

        Checkbox.labelElement.style.display = DisplayStyle.None;
        Checkbox.style.marginRight = 10;

        var checkmark = Checkbox.Q(className: "unity-toggle__checkmark");

        if (checkmark != null)
        {
            checkmark.style.width = 18;
            checkmark.style.height = 18;

            checkmark.style.backgroundColor = new Color(0.14f, 0.14f, 0.14f);

            checkmark.style.borderTopWidth = 1;
            checkmark.style.borderBottomWidth = 1;
            checkmark.style.borderLeftWidth = 1;
            checkmark.style.borderRightWidth = 1;

            checkmark.style.borderTopColor = new Color(0.35f, 0.35f, 0.35f);
            checkmark.style.borderBottomColor = new Color(0.35f, 0.35f, 0.35f);
            checkmark.style.borderLeftColor = new Color(0.35f, 0.35f, 0.35f);
            checkmark.style.borderRightColor = new Color(0.35f, 0.35f, 0.35f);

            checkmark.style.borderTopLeftRadius = 4;
            checkmark.style.borderTopRightRadius = 4;
            checkmark.style.borderBottomLeftRadius = 4;
            checkmark.style.borderBottomRightRadius = 4;
        }

        Checkbox.RegisterValueChangedCallback(evt =>
        {
            if (checkmark != null)
            {
                checkmark.style.backgroundColor = evt.newValue
                    ? new Color(0.20f, 0.55f, 1.0f)
                    : new Color(0.14f, 0.14f, 0.14f);
            }

            UpdateFieldState(evt.newValue);
        });

        // VALUE CONTAINER
        _valueContainer = new VisualElement();
        _valueContainer.style.flexDirection = FlexDirection.Row;
        _valueContainer.style.alignItems = Align.Center;

        // FLOAT FIELD
        ValueField = new FloatField();
        ValueField.value = 3.2f;
        ValueField.isDelayed = true;

        StyleInputField(ValueField);

        var textInput = ValueField.Q(className: "unity-text-input");

        if (textInput != null)
        {
            textInput.style.backgroundColor = Color.clear;
            textInput.style.color = Color.white;

            textInput.style.borderTopWidth = 0;
            textInput.style.borderBottomWidth = 0;
            textInput.style.borderLeftWidth = 0;
            textInput.style.borderRightWidth = 0;

            textInput.style.paddingLeft = 8;
        }

        // OBJECT FIELD
        ObjectField = new ObjectField();
        ObjectField.objectType = typeof(GameObject);

        StyleInputField(ObjectField);

        // UNIT LABEL
        _unitLabel = new Label("m");
        _unitLabel.style.marginLeft = 4;
        _unitLabel.style.fontSize = 12;
        _unitLabel.style.color = new Color(0.7f, 0.7f, 0.7f);

        // LEFT SIDE
        var leftContainer = new VisualElement();
        leftContainer.style.flexDirection = FlexDirection.Column;
        leftContainer.style.justifyContent = Justify.Center;

        _titleLabel = new Label("Title");
        _titleLabel.style.fontSize = 15;
        _titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        _titleLabel.style.color = new Color(0.92f, 0.92f, 0.92f);

        _subtitleLabel = new Label("Default: 2.5");
        _subtitleLabel.style.fontSize = 12;
        _subtitleLabel.style.color = new Color(0.55f, 0.55f, 0.55f);
        _subtitleLabel.style.marginTop = 2;

        leftContainer.Add(_titleLabel);
        leftContainer.Add(_subtitleLabel);

        rightContainer.Add(Checkbox);
        rightContainer.Add(_valueContainer);

        Add(leftContainer);
        Add(rightContainer);

        RefreshFieldMode();
        UpdateFieldState(Checkbox.value);
    }

    private void RefreshFieldMode()
    {
        _valueContainer.Clear();

        switch (Mode)
        {
            case FieldMode.Float:
                _valueContainer.Add(ValueField);
                _valueContainer.Add(_unitLabel);
                break;

            case FieldMode.Object:
                _valueContainer.Add(ObjectField);
                break;
        }
    }

    private void UpdateFieldState(bool enabled)
    {
        ValueField.SetEnabled(enabled);
        ObjectField.SetEnabled(enabled);

        ValueField.style.opacity = enabled ? 1f : 0.5f;
        ObjectField.style.opacity = enabled ? 1f : 0.5f;
        _unitLabel.style.opacity = enabled ? 1f : 0.5f;
    }

    private void StyleInputField(VisualElement field)
    {
        field.style.width = 140;
        field.style.height = 32;

        field.style.backgroundColor = new Color(0.12f, 0.14f, 0.17f);

        field.style.borderTopWidth = 1;
        field.style.borderBottomWidth = 1;
        field.style.borderLeftWidth = 1;
        field.style.borderRightWidth = 1;

        field.style.borderTopColor = new Color(0.28f, 0.28f, 0.28f);
        field.style.borderBottomColor = new Color(0.28f, 0.28f, 0.28f);
        field.style.borderLeftColor = new Color(0.28f, 0.28f, 0.28f);
        field.style.borderRightColor = new Color(0.28f, 0.28f, 0.28f);

        field.style.borderTopLeftRadius = 6;
        field.style.borderTopRightRadius = 6;
        field.style.borderBottomLeftRadius = 6;
        field.style.borderBottomRightRadius = 6;
    }
}
