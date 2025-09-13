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

    [Foldout("Knight References", true)]
    [MustBeAssigned][SerializeField] GameObject KnightPrefab;
    [HideInInspector] public Transform KnightSpawnPosition;
    private Transform KnightActor;

    [Foldout("Cutscenes", true)]
    public List<PlayableAsset> Cutscenes;
    private int _cutsceneIndex;

    public void Awake()
    {
        SceneInitialize.Instance.Subscribe(Init);
    }

    private void Init()
    {
        // Spawn actors
        Ctx.SpawnPlayer();
        //Ctx.Init();
        //return;


        KnightActor = Instantiate(KnightPrefab, KnightSpawnPosition).transform;

        // Start opening cutscene
        VCam.Follow = Ctx.player.transform;

        _cutsceneIndex = -1;
        NextCutscene();
    }

    public void NextCutscene()
    {
        _cutsceneIndex++;
        Director.playableAsset = Cutscenes[_cutsceneIndex];
        Director.Play();
    }

    public void StartOpeningDialogue()
    {
        DialogueManager.StartConversation("Knight", Ctx.PlayerActor, KnightActor);
    }

    public void FinishKnightDialogue()
    {
        DialogueManager.StopConversation();
        NextCutscene();
    }

    public void StartBattle()
    {
        DialogueManager.StopConversation();
        Destroy(KnightActor.gameObject);
        Ctx.Init();
    }
}
