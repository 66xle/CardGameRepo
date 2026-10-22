#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Playables;
using UnityEngine.Animations;

[UxmlElement]
public partial class AnimationPreviewElement : VisualElement
{
    // =========================
    // PREVIEW CORE
    // =========================
    private PreviewRenderUtility preview;
    private GameObject previewInstance;
    private Animator animator;

    private PlayableGraph playableGraph;
    private AnimationClipPlayable clipPlayable;

    // =========================
    // DATA
    // =========================
    private AnimationClip clip;

    // =========================
    // UI
    // =========================
    private ObjectField prefabField;
    private ObjectField clipField;

    private Button playButton;
    private Button frameButton;
    private Toggle loopToggle;
    private Slider timeline;

    private IMGUIContainer container;

    // =========================
    // STATE
    // =========================
    private bool playing;
    private float time;

    // camera
    private float zoom = 1f;
    private float rotX = 15f;
    private float rotY = 180f;
    private Vector3 pan;

    public AnimationPreviewElement()
    {
        style.flexGrow = 1;

        // ---------------- PREFAB ----------------
        prefabField = new ObjectField("Prefab")
        {
            objectType = typeof(GameObject),
            allowSceneObjects = false
        };

        prefabField.RegisterValueChangedCallback(e =>
        {
            LoadPrefab(e.newValue as GameObject);
        });

        Add(prefabField);

        // ---------------- CLIP ----------------
        clipField = new ObjectField("Animation Clip")
        {
            objectType = typeof(AnimationClip),
            allowSceneObjects = false
        };

        clipField.RegisterValueChangedCallback(e =>
        {
            SetClip(e.newValue as AnimationClip);
        });

        Add(clipField);

        // ---------------- TOOLBAR ----------------
        var toolbar = new Toolbar();

        playButton = new Button(TogglePlay) { text = "Play" };
        frameButton = new Button(Frame) { text = "Frame" };
        loopToggle = new Toggle("Loop") { value = true };

        toolbar.Add(playButton);
        toolbar.Add(frameButton);
        toolbar.Add(loopToggle);

        Add(toolbar);

        // ---------------- TIMELINE ----------------
        timeline = new Slider(0, 1);

        timeline.RegisterValueChangedCallback(e =>
        {
            if (clip == null) return;

            time = e.newValue * clip.length;
            Evaluate();
        });

        Add(timeline);

        // ---------------- IMGUI PREVIEW AREA ----------------
        container = new IMGUIContainer(OnIMGUI);
        container.style.flexGrow = 1;
        Add(container);

        RegisterCallback<AttachToPanelEvent>(_ =>
        {
            EditorApplication.update += Update;
        });

        RegisterCallback<DetachFromPanelEvent>(_ =>
        {
            EditorApplication.update -= Update;
            Cleanup();
        });
    }

    // =========================================================
    // LOAD PREFAB
    // =========================================================
    private void LoadPrefab(GameObject prefab)
    {
        CleanupInstance();

        if (prefab == null)
            return;

        preview = new PreviewRenderUtility();

        preview.cameraFieldOfView = 30f;
        preview.camera.clearFlags = CameraClearFlags.Color;
        preview.camera.backgroundColor = Color.gray;

        preview.lights[0].intensity = 1.5f;
        preview.lights[1].intensity = 1.2f;

        preview.lights[0].transform.rotation = Quaternion.Euler(40, 40, 0);
        preview.lights[1].transform.rotation = Quaternion.Euler(-40, -40, 0);

        previewInstance = Object.Instantiate(prefab);
        previewInstance.hideFlags = HideFlags.HideAndDontSave;

        previewInstance.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        previewInstance.transform.localScale = Vector3.one;

        animator = previewInstance.GetComponent<Animator>();
        if (animator == null)
            animator = previewInstance.AddComponent<Animator>();

        preview.AddSingleGO(previewInstance);

        Frame();
    }

    private void CleanupInstance()
    {
        if (playableGraph.IsValid())
            playableGraph.Destroy();

        if (previewInstance != null)
            Object.DestroyImmediate(previewInstance);

        previewInstance = null;
        animator = null;
        clip = null;
        time = 0f;
        playing = false;
    }

    // =========================================================
    // CLIP + PLAYABLE GRAPH
    // =========================================================
    private void SetClip(AnimationClip newClip)
    {
        clip = newClip;
        time = 0f;

        BuildGraph();
        Evaluate();
    }

    private void BuildGraph()
    {
        if (animator == null || clip == null)
            return;

        if (playableGraph.IsValid())
            playableGraph.Destroy();

        playableGraph = PlayableGraph.Create("AnimPreviewGraph");
        playableGraph.SetTimeUpdateMode(DirectorUpdateMode.Manual);

        var output =
            AnimationPlayableOutput.Create(playableGraph, "Anim", animator);

        clipPlayable =
            AnimationClipPlayable.Create(playableGraph, clip);

        output.SetSourcePlayable(clipPlayable);

        playableGraph.Play();
    }

    private void Evaluate()
    {
        if (!playableGraph.IsValid() || clipPlayable.IsNull() || clip == null)
            return;

        double t = time;

        if (loopToggle.value && clip.length > 0)
            t %= clip.length;

        clipPlayable.SetTime(t);
        playableGraph.Evaluate();
    }

    // =========================================================
    // UPDATE LOOP
    // =========================================================
    private void Update()
    {
        if (previewInstance == null || clip == null)
            return;

        if (playing)
        {
            time += Time.deltaTime;

            if (clip.length > 0)
            {
                if (loopToggle.value)
                    time %= clip.length;
                else
                    time = Mathf.Min(time, clip.length);
            }

            timeline.SetValueWithoutNotify(
                clip.length > 0 ? time / clip.length : 0);

            Evaluate();
        }

        MarkDirtyRepaint();
    }

    private void TogglePlay()
    {
        playing = !playing;
        playButton.text = playing ? "Pause" : "Play";
    }

    // =========================================================
    // IMGUI RENDER (FIXED)
    // =========================================================
    private void OnIMGUI()
    {
        if (preview == null || previewInstance == null)
            return;

        Rect r = GUILayoutUtility.GetRect(500, 300); ;

        if (r.width <= 1 || r.height <= 1)
            return;

        HandleInput(r);

        Frame();

        preview.BeginPreview(r, GUIStyle.none);
        preview.camera.Render();
        Texture tex = preview.EndPreview();
        GUI.DrawTexture(r, tex, ScaleMode.ScaleToFit, false);

        container.MarkDirtyRepaint();
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
            rotY += delta.x * 0.3f;  // rotation sensitivity X (yaw)
            rotX += delta.y * 0.3f;  // rotation sensitivity Y (pitch)
            rotX = Mathf.Clamp(rotX, -90f, 90f); // clamp pitch to avoid flipping
            e.Use();
        }

        if (e.type == EventType.MouseDrag && e.button == 2) // middle mouse
        {
            Vector2 delta = e.delta;
            // Adjust pan speed to taste
            float panSpeed = 0.005f;

            // Calculate right and up vectors relative to camera rotation for panning
            Quaternion rotation = Quaternion.Euler(rotX, rotY, 0);
            Vector3 right = rotation * Vector3.right;
            Vector3 up = rotation * Vector3.up;

            pan += (-right * delta.x + up * delta.y) * panSpeed;

            e.Use();
        }
    }

    // =========================================================
    // FRAME CAMERA (UNITY STYLE)
    // =========================================================
    private void Frame()
    {
        if (previewInstance == null || preview == null) return;

        Bounds b = CalculateBounds(previewInstance);

        Vector3 center = b.center + pan;

        float radius = b.extents.magnitude;
        float fov = preview.camera.fieldOfView;

        float distance = (radius / Mathf.Sin(Mathf.Deg2Rad * fov * 0.5f)) / zoom;

        UpdateClipping(distance);

        Quaternion rot = Quaternion.Euler(rotX, rotY, 0);
        Vector3 offset = rot * new Vector3(0, 0, -distance);
        preview.camera.transform.position = center + offset;
        preview.camera.transform.LookAt(center);
    }

    private void UpdateClipping(float distance)
    {
        preview.camera.nearClipPlane = Mathf.Max(0.01f, distance * 0.1f);
        preview.camera.farClipPlane = Mathf.Max(50f, distance * 10f);
    }

    // =========================================================
    // BOUNDS
    // =========================================================
    private static Bounds CalculateBounds(GameObject go)
    {
        var renderers = go.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
            return new Bounds(Vector3.zero, Vector3.one);

        Bounds b = renderers[0].bounds;

        for (int i = 1; i < renderers.Length; i++)
            b.Encapsulate(renderers[i].bounds);

        return b;
    }

    // =========================================================
    // CLEANUP
    // =========================================================
    private void Cleanup()
    {
        if (playableGraph.IsValid())
            playableGraph.Destroy();

        if (previewInstance != null)
            Object.DestroyImmediate(previewInstance);

        preview?.Cleanup();
    }
}

#endif
