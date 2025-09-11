using Cinemachine;
using MyBox;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [MustBeAssigned] [SerializeField] LevelSettings LevelSettings;
    [MustBeAssigned] [SerializeField] EnemyManager EnemyManager;
    [MustBeAssigned] [SerializeField] CombatStateMachine Ctx;
    [MustBeAssigned] [SerializeField] PlayableDirector Director;
    [MustBeAssigned] [SerializeField] CinemachineVirtualCamera VCam;

    [MustBeAssigned] [SerializeField] Transform EnvironmentParent;

    [HideInInspector] public bool isEnvironmentLoaded = false;

    private void Awake()
    {
        SceneInitialize.Instance.Clear();
        SceneInitialize.Instance.Subscribe(Init, -10);
        ServiceLocator.Register(this);
    }


    private void Init()
    {
        if (GameManager.Instance.LoadedEnvironment != null)
        {
            GameManager.Instance.LoadedEnvironment.transform.SetParent(EnvironmentParent);
            isEnvironmentLoaded = true;
        }

#if UNITY_EDITOR
        if (!isEnvironmentLoaded) return;
#endif

        GetAvatarPositions();
        // Spawn actors
        Ctx.SpawnPlayer();
        // Spawn knight

        // Start cutscene
        VCam.Follow = Ctx.player.transform;
        Director.Play();

        AudioData musicData = GameManager.Instance.CurrentLevelDataLoaded.Music;
        AudioManager.Instance.PlayMusic(musicData);
    }

    public void StartOpeningDialogue()
    {
        DialogueManager.StartConversation("Knight", Ctx.PlayerActor);
    }

    public void FinishKnightDialogue()
    {
        DialogueManager.StopConversation();
        Ctx.Init();
        //Debug.Log("Next cutscene");
    }

    void GetAvatarPositions()
    {
        AvatarSpawnPosition asp = ServiceLocator.Get<AvatarSpawnPosition>();
        EnemyManager.EnemySpawnPosList = asp.EnemySpawnPositionList;
        Ctx.PlayerSpawnPos = asp.PlayerSpawnPosition;
    }
}
