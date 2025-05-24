using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomPreviewHandler
{
    PreviewRenderUtility previewRenderUtility;
    GameObject previewInstance;

    float zoom = 1f;
    float rotationX = 20f;  // starting pitch
    float rotationY = 30f;  // starting yaw
    float baseDistance = 5f; // default camera distance from object
    float minNear = 0.1f;   // smallest near plane distance
    float maxFar = 1000f;   // far plane distance
    Vector3 panOffset = Vector3.zero;

    public void Init(GameObject prefab, int textureSize = 512)
    {
        Cleanup();

        previewRenderUtility = new PreviewRenderUtility();
        previewRenderUtility.cameraFieldOfView = 30f;
        previewRenderUtility.camera.clearFlags = CameraClearFlags.Color;
        previewRenderUtility.camera.backgroundColor = Color.gray;
        previewRenderUtility.camera.transform.position = new Vector3(0, 0, -5);
        previewRenderUtility.camera.transform.LookAt(Vector3.zero);

        previewInstance = GameObject.Instantiate(prefab);
        previewInstance.hideFlags = HideFlags.HideAndDontSave;
        previewRenderUtility.AddSingleGO(previewInstance);

        // Position camera so prefab is visible
        FrameObject();
    }

    public void Cleanup()
    {
        if (previewInstance != null)
        {
            GameObject.DestroyImmediate(previewInstance);
            previewInstance = null;
        }
        if (previewRenderUtility != null)
        {
            previewRenderUtility.Cleanup();
            previewRenderUtility = null;
        }
    }

    public void OnPreviewGUI(Rect rect)
    {
        if (previewRenderUtility == null)
            return;

        HandleInput(rect);

        FrameObject();

        // Render preview
        previewRenderUtility.BeginPreview(rect, GUIStyle.none);
        previewRenderUtility.camera.Render();
        Texture tex = previewRenderUtility.EndPreview();

        GUI.DrawTexture(rect, tex, ScaleMode.ScaleToFit, false);
    }

    private void HandleInput(Rect r)
    {
        UnityEngine.Event e = UnityEngine.Event.current;
        if (!r.Contains(e.mousePosition)) return;

        // Handle zoom with scroll wheel
        if (e.type == EventType.ScrollWheel)
        {
            float zoomDelta = -e.delta.y * 0.05f;  // Adjust zoom speed here
            zoom = Mathf.Clamp(zoom + zoomDelta, 0.3f, 3f);
            e.Use();
        }

        // Handle rotation with left mouse drag
        if (e.type == EventType.MouseDrag && e.button == 0)
        {
            Vector2 delta = e.delta;
            rotationY += delta.x * 0.3f;  // rotation sensitivity X (yaw)
            rotationX += delta.y * 0.3f;  // rotation sensitivity Y (pitch)
            rotationX = Mathf.Clamp(rotationX, -90f, 90f); // clamp pitch to avoid flipping
            e.Use();
        }

        if (e.type == EventType.MouseDrag && e.button == 2) // middle mouse
        {
            Vector2 delta = e.delta;
            // Adjust pan speed to taste
            float panSpeed = 0.005f;

            // Calculate right and up vectors relative to camera rotation for panning
            Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);
            Vector3 right = rotation * Vector3.right;
            Vector3 up = rotation * Vector3.up;

            panOffset += (-right * delta.x + up * delta.y) * panSpeed;

            e.Use();
        }
    }

    void UpdateCameraClippingPlanes(float distance)
    {
        // Make near plane smaller when zoomed in close
        previewRenderUtility.camera.nearClipPlane = Mathf.Max(minNear, distance * 0.1f);

        // Make far plane large enough to see the whole prefab
        previewRenderUtility.camera.farClipPlane = Mathf.Max(distance * 3f, maxFar);
    }

    void FrameObject()
    {
        if (previewInstance == null || previewRenderUtility == null) return;

        Bounds bounds = CalculateBounds(previewInstance);
        Vector3 center = bounds.center + panOffset;

        float radius = bounds.extents.magnitude;
        float distance = radius / Mathf.Sin(Mathf.Deg2Rad * previewRenderUtility.camera.fieldOfView / 2f);
        distance /= zoom;

        UpdateCameraClippingPlanes(distance);

        Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);
        Vector3 offset = rotation * new Vector3(0, 0, -distance);
        previewRenderUtility.camera.transform.position = center + offset;
        previewRenderUtility.camera.transform.LookAt(center);
    }

    Bounds CalculateBounds(GameObject go)
    {
        Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            return new Bounds(go.transform.position, Vector3.one);

        Bounds b = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
            b.Encapsulate(renderers[i].bounds);
        return b;
    }

    
}
