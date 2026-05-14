using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class LabelCheckboxField : VisualElement
{
    // Elements
    public Toggle Checkbox { get; private set; }
    public FloatField ValueField { get; private set; }

    private readonly Label _titleLabel;
    private readonly Label _subtitleLabel;
    private readonly Label _unitLabel;

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

    public float Value
    {
        get => ValueField.value;
        set => ValueField.value = value;
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

        // Force checkbox appearance
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

        // VALUE FIELD
        var valueContainer = new VisualElement();
        valueContainer.style.flexDirection = FlexDirection.Row;
        valueContainer.style.alignItems = Align.Center;

        ValueField = new FloatField();
        ValueField.value = 3.2f;
        ValueField.isDelayed = true;

        ValueField.style.width = 80;
        ValueField.style.height = 32;

        ValueField.style.backgroundColor = new Color(0.12f, 0.14f, 0.17f);

        ValueField.style.borderTopWidth = 1;
        ValueField.style.borderBottomWidth = 1;
        ValueField.style.borderLeftWidth = 1;
        ValueField.style.borderRightWidth = 1;

        ValueField.style.borderTopColor = new Color(0.28f, 0.28f, 0.28f);
        ValueField.style.borderBottomColor = new Color(0.28f, 0.28f, 0.28f);
        ValueField.style.borderLeftColor = new Color(0.28f, 0.28f, 0.28f);
        ValueField.style.borderRightColor = new Color(0.28f, 0.28f, 0.28f);

        ValueField.style.borderTopLeftRadius = 6;
        ValueField.style.borderTopRightRadius = 6;
        ValueField.style.borderBottomLeftRadius = 6;
        ValueField.style.borderBottomRightRadius = 6;

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

        _subtitleLabel = new Label($"Default: 2.5");
        _subtitleLabel.style.fontSize = 12;
        _subtitleLabel.style.color = new Color(0.55f, 0.55f, 0.55f);
        _subtitleLabel.style.marginTop = 2;

        leftContainer.Add(_titleLabel);
        leftContainer.Add(_subtitleLabel);


        valueContainer.Add(ValueField);
        valueContainer.Add(_unitLabel);

        rightContainer.Add(Checkbox);
        rightContainer.Add(valueContainer);

        Add(leftContainer);
        Add(rightContainer);

        UpdateFieldState(Checkbox.value);
    }

    private void UpdateFieldState(bool enabled)
    {
        ValueField.SetEnabled(enabled);

        ValueField.style.opacity = enabled ? 1f : 0.5f;
        _unitLabel.style.opacity = enabled ? 1f : 0.5f;
    }
}
