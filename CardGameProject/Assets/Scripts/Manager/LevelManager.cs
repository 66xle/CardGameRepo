using MyBox;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [MustBeAssigned] [SerializeField] LevelSettings LevelSettings;
    [MustBeAssigned] [SerializeField] EnemyManager EnemyManager;
    [MustBeAssigned] [SerializeField] CombatStateMachine Ctx;

    [MustBeAssigned] [SerializeField] Transform EnvironmentParent;

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
        if (GameManager.Instance.LoadedEnvironment != null)
        {
            GameManager.Instance.LoadedEnvironment.transform.SetParent(EnvironmentParent);
            isEnvironmentLoaded = true;
        }

#if UNITY_EDITOR
        if (!isEnvironmentLoaded) return;
#endif


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
