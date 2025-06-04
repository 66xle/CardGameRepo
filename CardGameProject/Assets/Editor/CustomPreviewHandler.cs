using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class CustomPreviewHandler
{
    PreviewRenderUtility previewRenderUtility;
    GameObject previewInstance;
    Animator animator;
    AnimationClipData clipData;
    float animationSpeed = 1f;
    float animationTime;

    private PlayableGraph playableGraph;
    private AnimationClipPlayable clipPlayable;

    float zoom = 1f;
    float rotationX = 20f;  // starting pitch
    float rotationY = 30f;  // starting yaw
    float baseDistance = 5f; // default camera distance from object
    float minNear = 0.1f;   // smallest near plane distance
    float maxFar = 1000f;   // far plane distance
    Vector3 panOffset = Vector3.zero;

    Bounds cachedBounds;


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

        animator = previewInstance.GetComponent<Animator>();
        if (animator == null)
            animator = previewInstance.AddComponent<Animator>();

        

        cachedBounds = CalculateBounds(previewInstance);

        // Position camera so prefab is visible
        FrameObject();
    }

    public void Cleanup()
    {
        if (playableGraph.IsValid())
            playableGraph.Destroy();

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

        if (playableGraph.IsValid() && (clipData != null))
        {
            if (clipData.clip.length > 0)
            {
                if (animationSpeed > 0f)
                {
                    animationTime += Time.deltaTime * animationSpeed;
                    animationTime %= clipData.clip.length;
                }

                clipPlayable.SetTime(animationTime);
                playableGraph.Evaluate(); // Important!
            }
        }

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

        Bounds bounds = cachedBounds;
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

    public void PlayClip()
    {
        if (clipData == null) return;

        // Playable Graph setup
        playableGraph = PlayableGraph.Create("PreviewGraph");
        var playableOutput = AnimationPlayableOutput.Create(playableGraph, "AnimOutput", animator);

        animationSpeed = 0.5f;

        clipData.clip.wrapMode = WrapMode.Loop; // Ensures the clip itself is loopable
        clipPlayable = AnimationClipPlayable.Create(playableGraph, clipData.clip);
        playableOutput.SetSourcePlayable(clipPlayable);
        playableGraph.Play();
    }

    public void PauseClip()
    {
        animationSpeed = 0f;
    }

    public void SetClipData(AnimationClipData clipData)
    {
        this.clipData = clipData;
    }

    public bool IsClipEmpty()
    {
        if (clipData == null)
            return true;

        return false;
    }
        
}
