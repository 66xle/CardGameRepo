using MyBox;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [MustBeAssigned] [SerializeField] LevelSettings LevelSettings;
    [MustBeAssigned] [SerializeField] EnemyManager EnemyManager;
    [MustBeAssigned] [SerializeField] CutsceneManager CutsceneManager;
    [MustBeAssigned] [SerializeField] CombatStateMachine Ctx;

    [MustBeAssigned] [SerializeField] Transform EnvironmentParent;

    [HideInInspector] public bool isEnvironmentLoaded = false;

    private void Awake()
    {
        SceneInitialize.Instance.Subscribe(Init, -10);
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
    }

    void GetAvatarPositions()
    {
        AvatarSpawnPosition asp = ServiceLocator.Get<AvatarSpawnPosition>();
        EnemyManager.EnemySpawnPosList = asp.EnemySpawnPositionList;
        Ctx.PlayerSpawnPos = asp.PlayerSpawnPosition;
        CutsceneManager.KnightSpawnPosition = asp.KnightSpawnPosition;
    }
}
