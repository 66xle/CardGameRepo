using Cinemachine;
using MyBox;
using PixelCrushers.DialogueSystem;
using System;
using System.Collections.Generic;
using Systems.SceneManagment;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


public class CutsceneManager : MonoBehaviour
{
    [Foldout("References", true)]
    [MustBeAssigned][SerializeField] CombatStateMachine Ctx;
    [MustBeAssigned][SerializeField] GameObject Canvas;
    [MustBeAssigned][SerializeField] Camera MainCamera;


    [Foldout("Knight References", true)]
    [MustBeAssigned][SerializeField] GameObject KnightPrefab;
    [HideInInspector] public Transform KnightSpawnPosition;

    [Foldout("Cutscenes", true)]
    public bool RunSignal;
    public SignalAsset SignalAsset;
    public List<GameObject> Cutscenes;
    private List<(GameObject, PlayableDirector)> LoadedCutscenes = new();
    private int _cutsceneIndex;
    private AvatarSpawnPosition asp;


    private List<GameObject> cutsceneObjects = new();

    [ButtonMethod]
    public void RunSignalMethod()
    {
        SignalReceiver signalReciver = GetComponent<SignalReceiver>();
        signalReciver.GetReaction(SignalAsset)?.Invoke();
    }
    
    public void Awake()
    {
        ServiceLocator.Register(this);

        SceneInitialize.Instance.Subscribe(Init);
    }

    public void OnDestroy()
    {
        ServiceLocator.Unregister<CutsceneManager>();
    }


    private void Init()
    {
        _cutsceneIndex = -1;

        asp = ServiceLocator.Get<AvatarSpawnPosition>();

        if (RunSignal)
        {
            SignalReceiver signalReciver = GetComponent<SignalReceiver>();
            signalReciver.GetReaction(SignalAsset)?.Invoke();
            return;
        }

        NextCutscene();
    }

    public void PlaySignal(SignalAsset signal)
    {
        SignalReceiver signalReciver = GetComponent<SignalReceiver>();
        signalReciver.GetReaction(signal)?.Invoke();
    }

    public void PreloadNextCutscene()
    {
        GameObject loadedCutscene = SpawnCutscene();
        loadedCutscene.SetActive(false);
        
        PlayableDirector director = loadedCutscene.GetComponentInChildren<PlayableDirector>();
        director.time = 0;
        director.Evaluate(); // Prewarm
        director.Stop();

        LoadedCutscenes.Add((loadedCutscene, director));
    }


    public void PlayPreloadedCutscene()
    { 
        DisableAudioListener();
        
        GameObject prefab = LoadedCutscenes[0].Item1;
        PlayableDirector director = LoadedCutscenes[0].Item2;

        prefab.SetActive(true);
        director.Play();
    }

    public void NextCutscene()
    {
        GameObject loadedCutscene = SpawnCutscene();

        PlayableDirector director = loadedCutscene.GetComponentInChildren<PlayableDirector>();
        director.time = 0;
        director.Play();
        LoadedCutscenes.Add((loadedCutscene, director));

        DisableAudioListener();
    }

    public GameObject SpawnCutscene()
    {
        _cutsceneIndex++;

        GameObject prefab = Cutscenes[_cutsceneIndex];
        GameObject loadedCutscene = Instantiate(prefab, asp.CutsceneParent);

        return loadedCutscene;
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

    public void StopConversation()
    {
        DialogueManager.StopAllConversations();
    }

    private void DestroyPrefab()
    {
        if (LoadedCutscenes.Count > 0)
        {
            GameObject loadedCutscene = LoadedCutscenes[0].Item1;
            Destroy(loadedCutscene);
            LoadedCutscenes.RemoveAt(0);
        }
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

    public void ResumeTower()
    {
        asp.Endless.ResumeTunnel();
        
    }

    public void TowerCutscene()
    {
        asp.Endless.GetComponentInChildren<TunnelCutsceneHelper>().PlayCutscene();
    }
}
