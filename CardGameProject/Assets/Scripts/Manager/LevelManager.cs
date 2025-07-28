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
        _currentLevel = GameManager.Instance.StageLevel;

        LoadLevel();
    }

    void LoadLevel()
    {
        if (_currentLevel >= LevelSettings.Levels.Count)
            _currentLevel = LevelSettings.Levels.Count - 1;

        LevelData data = LevelSettings.Levels[_currentLevel];

        GameObject environment = Instantiate(data.Prefab);

        AvatarSpawnPosition asp = environment.GetComponent<AvatarSpawnPosition>();
        EnemyManager.EnemySpawnPosList = asp.EnemySpawnPositionList;
        Ctx.PlayerSpawnPos = asp.PlayerSpawnPosition;
    }
}
