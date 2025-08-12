using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.UI;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System.Linq;

public class PopupWindow : EditorWindow
{
    public BaseEditorWindow window;

    public bool addButtonPressed = false;
    public bool renameButtonPressed = false;

    private void OnDisable()
    {
        if (window != null)
            window.isPopupActive = false;
    }

}
