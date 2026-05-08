using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

[UxmlElement]
public partial class GearCardElement : VisualElement
{
    private readonly VisualElement _radioOuter;
    private readonly VisualElement _radioInner;
    private readonly Image _icon;
    private readonly Label _title;

    private bool _selected;

    public bool Selected
    {
        get => _selected;
        set
        {
            _selected = value;
            RefreshState();
        }
    }

    public string Title
    {
        get => _title.text;
        set => _title.text = value;
    }

    public Sprite Icon
    {
        set => _icon.image = value.texture;
    }

    public GearCardElement()
    {
        style.flexDirection = FlexDirection.Row;
        style.alignItems = Align.Center;
        style.height = 72;
        style.marginLeft = 8;
        style.marginRight = 16;

        style.paddingLeft = 14;
        style.paddingRight = 14;

        style.borderTopLeftRadius = 12;
        style.borderTopRightRadius = 12;
        style.borderBottomLeftRadius = 12;
        style.borderBottomRightRadius = 12;

        style.borderTopWidth = 1;
        style.borderBottomWidth = 1;
        style.borderLeftWidth = 1;
        style.borderRightWidth = 1;

        style.backgroundColor = new Color(0.07f, 0.08f, 0.11f);
        style.borderTopColor = new Color(0.18f, 0.2f, 0.28f);
        style.borderBottomColor = new Color(0.18f, 0.2f, 0.28f);
        style.borderLeftColor = new Color(0.18f, 0.2f, 0.28f);
        style.borderRightColor = new Color(0.18f, 0.2f, 0.28f);

        // Radio Circle
        _radioOuter = new VisualElement();
        _radioOuter.style.width = 20;
        _radioOuter.style.height = 20;
        _radioOuter.style.borderTopLeftRadius = 10;
        _radioOuter.style.borderTopRightRadius = 10;
        _radioOuter.style.borderBottomLeftRadius = 10;
        _radioOuter.style.borderBottomRightRadius = 10;

        _radioOuter.style.borderTopWidth = 2;
        _radioOuter.style.borderBottomWidth = 2;
        _radioOuter.style.borderLeftWidth = 2;
        _radioOuter.style.borderRightWidth = 2;

        _radioOuter.style.justifyContent = Justify.Center;
        _radioOuter.style.alignItems = Align.Center;

        _radioInner = new VisualElement();
        _radioInner.style.width = 8;
        _radioInner.style.height = 8;
        _radioInner.style.borderTopLeftRadius = 4;
        _radioInner.style.borderTopRightRadius = 4;
        _radioInner.style.borderBottomLeftRadius = 4;
        _radioInner.style.borderBottomRightRadius = 4;

        _radioOuter.Add(_radioInner);

        // Icon
        _icon = new Image();
        _icon.scaleMode = ScaleMode.ScaleToFit;
        _icon.style.width = 42;
        _icon.style.height = 42;
        _icon.style.marginLeft = 14;
        _icon.style.marginRight = 14;

        // Title
        _title = new Label("Card Name");
        _title.style.unityFontStyleAndWeight = FontStyle.Bold;
        _title.style.fontSize = 16;
        _title.style.color = Color.white;
        _title.style.flexGrow = 1;
        _title.name = "card-title";

        Add(_radioOuter);
        Add(_icon);
        Add(_title);

        RefreshState();
    }

    private void RefreshState()
    {
        if (_selected)
        {
            style.backgroundColor = new Color(0.09f, 0.1f, 0.16f);

            style.borderTopColor = new Color(0.38f, 0.35f, 0.95f);
            style.borderBottomColor = new Color(0.38f, 0.35f, 0.95f);
            style.borderLeftColor = new Color(0.38f, 0.35f, 0.95f);
            style.borderRightColor = new Color(0.38f, 0.35f, 0.95f);

            _radioOuter.style.borderTopColor = new Color(0.38f, 0.35f, 0.95f);
            _radioOuter.style.borderBottomColor = new Color(0.38f, 0.35f, 0.95f);
            _radioOuter.style.borderLeftColor = new Color(0.38f, 0.35f, 0.95f);
            _radioOuter.style.borderRightColor = new Color(0.38f, 0.35f, 0.95f);

            _radioInner.style.backgroundColor = new Color(0.38f, 0.35f, 0.95f);
            _radioInner.style.display = DisplayStyle.Flex;
        }
        else
        {
            style.backgroundColor = new Color(0.07f, 0.08f, 0.11f);

            style.borderTopColor = new Color(0.18f, 0.2f, 0.28f);
            style.borderBottomColor = new Color(0.18f, 0.2f, 0.28f);
            style.borderLeftColor = new Color(0.18f, 0.2f, 0.28f);
            style.borderRightColor = new Color(0.18f, 0.2f, 0.28f);

            _radioOuter.style.borderTopColor = new Color(0.28f, 0.3f, 0.38f);
            _radioOuter.style.borderBottomColor = new Color(0.28f, 0.3f, 0.38f);
            _radioOuter.style.borderLeftColor = new Color(0.28f, 0.3f, 0.38f);
            _radioOuter.style.borderRightColor = new Color(0.28f, 0.3f, 0.38f);

            _radioInner.style.display = DisplayStyle.None;
        }
    }
}
