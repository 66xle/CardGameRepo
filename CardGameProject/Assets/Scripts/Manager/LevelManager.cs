using MyBox;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [MustBeAssigned] [SerializeField] LevelSettings LevelSettings;
    [MustBeAssigned] [SerializeField] EnemyManager EnemyManager;
    [MustBeAssigned] [SerializeField] CombatStateMachine Ctx;

    [HideInInspector] public bool isEnvironmentLoaded = false;

    public void Awake()
    {
#if UNITY_EDITOR
        Scene scene = SceneManager.GetSceneByName("LoadingScene");
        if (scene.isLoaded)
        {
            isEnvironmentLoaded = true;
        }

        if (!isEnvironmentLoaded) return;
#endif

        GetAvatarPositions();
    }

    void GetAvatarPositions()
    {
        AvatarSpawnPosition asp = ServiceLocator.Get<AvatarSpawnPosition>();
        EnemyManager.EnemySpawnPosList = asp.EnemySpawnPositionList;
        Ctx.PlayerSpawnPos = asp.PlayerSpawnPosition;
    }
}
