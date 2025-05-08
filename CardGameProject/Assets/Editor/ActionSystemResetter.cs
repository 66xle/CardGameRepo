using UnityEditor;
using UnityEngine;

[InitializeOnLoad]  // This ensures that the class gets initialized as soon as the editor loads
public class ActionSystemResetter
{
    static ActionSystemResetter()
    {
        // Subscribe to the play mode state change event
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    // This will be called when the play mode state changes
    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            // Ensure the ActionSystem singleton is set to null when exiting play mode
            if (ActionSystem.Instance != null)
            {
                ActionSystem.Instance.Reset();
                Debug.Log("ActionSystem singleton set to null on play exit.");
            }
        }
    }
}