using MyBox;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [MustBeAssigned] [SerializeField] LevelSettings LevelSettings;
    [MustBeAssigned] [SerializeField] EnemyManager EnemyManager;
    [MustBeAssigned] [SerializeField] CombatStateMachine Ctx;

    [HideInInspector] public bool isEnvironmentLoaded = false;

    private void Awake()
    {
        SceneInitialize.Instance.Subscribe(Init, -10);
    }

    private void Start()
    {
        SceneInitialize.Instance.Invoke();
    }

    private void Init()
    {
        GetAvatarPositions();

        AudioData musicData = GameManager.Instance.CurrentLevelDataLoaded.Music;
        AudioManager.Instance.PlayMusic(musicData);
    }

    void GetAvatarPositions()
    {
        AvatarSpawnPosition asp = ServiceLocator.Get<AvatarSpawnPosition>();
        EnemyManager.EnemySpawnPosList = asp.EnemySpawnPositionList;
        Ctx.PlayerSpawnPos = asp.PlayerSpawnPosition;
    }
}
