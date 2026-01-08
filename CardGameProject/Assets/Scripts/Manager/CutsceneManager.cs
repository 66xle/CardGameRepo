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
    private List<(GameObject, PlayableDirector)> PreloadedCutscenes = new();
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

        SceneInitialize.Instance.Subscribe(Init, -7);
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


        PreloadNextCutscene();
        PreloadNextCutscene();
        PlayNextCutscene();
    }

    public void PlaySignal(SignalAsset signal)
    {
        SignalReceiver signalReciver = GetComponent<SignalReceiver>();
        signalReciver.GetReaction(signal)?.Invoke();
    }

    public void PreloadNextCutscene()
    {
        _cutsceneIndex++;

        GameObject prefab = Cutscenes[_cutsceneIndex];
        GameObject loadedCutscene = Instantiate(prefab, asp.CutsceneParent);
        
        PlayableDirector director = loadedCutscene.GetComponentInChildren<PlayableDirector>();
        director.time = 0;
        director.Evaluate(); // Prewarm
        director.Stop();

        PreloadedCutscenes.Add((loadedCutscene, director));
    }

    public void PlayNextCutscene()
    { 
        DisableAudioListener();
        
        GameObject prefab = PreloadedCutscenes[0].Item1;
        PlayableDirector director = PreloadedCutscenes[0].Item2;

        prefab.SetActive(true);
        director.Play();
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
        GameObject loadedCutscene = PreloadedCutscenes[0].Item1;
        Destroy(loadedCutscene);
        PreloadedCutscenes.RemoveAt(0);
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
