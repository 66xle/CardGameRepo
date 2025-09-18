using Cinemachine;
using MyBox;
using PixelCrushers.DialogueSystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneManager : MonoBehaviour
{
    [Foldout("References", true)]
    [MustBeAssigned][SerializeField] CombatStateMachine Ctx;
    [MustBeAssigned][SerializeField] CinemachineVirtualCamera VCam;
    [MustBeAssigned][SerializeField] PlayableDirector Director;
    [MustBeAssigned][SerializeField] GameObject Canvas;
    [MustBeAssigned][SerializeField] Camera MainCamera;
    [MustBeAssigned][SerializeField] Camera CutsceneCamera;

    [Foldout("Knight References", true)]
    [MustBeAssigned][SerializeField] GameObject KnightPrefab;
    [HideInInspector] public Transform KnightSpawnPosition;
    private Transform PlayerActor;
    private Transform KnightActor;

    [Foldout("Cutscenes", true)]
    public List<PlayableAsset> Cutscenes;
    private int _cutsceneIndex;
    private AvatarSpawnPosition asp;

    private List<GameObject> cutsceneObjects = new();

    public void Awake()
    {
        SceneInitialize.Instance.Subscribe(Init);
    }

    private void Init()
    {
        asp = ServiceLocator.Get<AvatarSpawnPosition>();

        // Spawn actors
        PlayerActor = SpawnGameObject(Ctx.PlayerPrefab, Ctx.PlayerSpawnPos).transform;
        KnightActor = SpawnGameObject(KnightPrefab, KnightSpawnPosition).transform;

        _cutsceneIndex = -1;
        NextCutscene();
    }

    public void NextCutscene()
    {
        EnableCutsceneCamera();

        _cutsceneIndex++;
        Director.playableAsset = Cutscenes[_cutsceneIndex];
        Director.Play();
    }

    public void EndCutscene()
    {
        RemoveCutsceneObjects();
        DisableCutsceneCamera();
    }

    public void EnableCutsceneCamera()
    {
        MainCamera.GetComponent<AudioListener>().enabled = false;
        CutsceneCamera.GetComponent<AudioListener>().enabled = true;
        CutsceneCamera.depth = 10;
    }

    public void DisableCutsceneCamera()
    {
        CutsceneCamera.GetComponent<AudioListener>().enabled = false;
        MainCamera.GetComponent<AudioListener>().enabled = true;
        CutsceneCamera.depth = 3;
    }

    public void StartKnightConversation()
    {
        DialogueManager.StartConversation("Knight", PlayerActor, KnightActor);
    }

    private void RemoveCutsceneObjects()
    {
        foreach (GameObject gameObject in cutsceneObjects)
        {
            Destroy(gameObject);
        }

        cutsceneObjects.Clear();
    }

    public void StartCombatStateMachine()
    {
        Ctx.Init();
    }

    public void SpawnActor(string actorName)
    {
        foreach (SpawnPosition data in asp.CutsceneSpawnPositions)
        {
            if (data.Name != actorName) continue;

            SpawnGameObject(data.Prefab, data.Transform);
        }
    }

    public GameObject SpawnGameObject(GameObject gameObject, Transform parent)
    {
        GameObject spawnedObject = Instantiate(gameObject, parent);
        spawnedObject.SetLayerRecursively(LayerMask.NameToLayer("Cutscene"));

        cutsceneObjects.Add(spawnedObject);

        return spawnedObject;
    }
}
