using Cinemachine;
using MyBox;
using PixelCrushers.DialogueSystem;
using System.Collections.Generic;
using Systems.SceneManagment;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


public class CutsceneManager : MonoBehaviour
{
    [Foldout("References", true)]
    [MustBeAssigned][SerializeField] CombatStateMachine Ctx;
    [MustBeAssigned][SerializeField] CinemachineVirtualCamera VCam;
    [MustBeAssigned][SerializeField] GameObject Canvas;
    [MustBeAssigned][SerializeField] Camera MainCamera;


    [Foldout("Knight References", true)]
    [MustBeAssigned][SerializeField] GameObject KnightPrefab;
    [HideInInspector] public Transform KnightSpawnPosition;
    private Transform PlayerActor;
    private Transform KnightActor;

    [Foldout("Cutscenes", true)]
    public bool RunSignal;
    public SignalAsset SignalAsset;
    public List<GameObject> Cutscenes;
    private int _cutsceneIndex;
    private AvatarSpawnPosition asp;

    private List<GameObject> cutsceneObjects = new();
    private GameObject _loadedPrefab;

    [ButtonMethod]
    public void RunSignalMethod()
    {
        SignalReceiver signalReciver = GetComponent<SignalReceiver>();
        signalReciver.GetReaction(SignalAsset)?.Invoke();
    }

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

        if (RunSignal)
        {
            SignalReceiver signalReciver = GetComponent<SignalReceiver>();
            signalReciver.GetReaction(SignalAsset)?.Invoke();
            return;
        }

        NextCutscene();
    }

    public void NextCutscene()
    { 
        DisableAudioListener();

        _cutsceneIndex++;
        GameObject prefab = Cutscenes[_cutsceneIndex];
        _loadedPrefab = Instantiate(prefab);
    }

    public void EndCutscene()
    {
        DestroyPrefab();
        EnableAudioListener();
    }

    public void DisableAudioListener()
    {
        MainCamera.GetComponent<AudioListener>().enabled = false;
    }

    public void EnableAudioListener()
    {
        MainCamera.GetComponent<AudioListener>().enabled = true;
    }

    public async void LoadMainMenu()
    {
        SceneLoader loader = ServiceLocator.Get<SceneLoader>();
        await loader.LoadSceneGroup("MainMenu");
    }

    public void StartKnightConversation()
    {
        DialogueManager.StartConversation("Knight");
    }

    private void DestroyPrefab()
    {
        Destroy(_loadedPrefab);
    }

    public void DestroyActors()
    {
        foreach (GameObject actor in cutsceneObjects)
        {
            Destroy(actor);
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
        //spawnedObject.SetLayerRecursively(LayerMask.NameToLayer("Cutscene"));

        cutsceneObjects.Add(spawnedObject);

        return spawnedObject;
    }
}
