using MyBox;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

public class LevelManager : MonoBehaviour
{
    [MustBeAssigned] [SerializeField] LevelSettings LevelSettings;
    [MustBeAssigned] [SerializeField] EnemyManager EnemyManager;
    [MustBeAssigned] [SerializeField] CombatStateMachine Ctx;

    private int _currentLevel;

    public void Awake()
    {
        _currentLevel++;
        GameManager.Instance.Level = _currentLevel;
        LoadLevel();
    }

    void LoadLevel()
    {
        LevelData data = LevelSettings.Levels.Count >= _currentLevel ? LevelSettings.Levels[LevelSettings.Levels.Count - 1] : LevelSettings.Levels[_currentLevel - 1];

        GameObject environment = Instantiate(data.Prefab);

        AvatarSpawnPosition asp = environment.GetComponent<AvatarSpawnPosition>();
        EnemyManager.EnemySpawnPosList = asp.EnemySpawnPositionList;
        Ctx.PlayerSpawnPos = asp.PlayerSpawnPosition;
    }
}
