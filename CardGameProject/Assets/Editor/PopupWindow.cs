using UnityEditor;

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
