using UnityEngine;
using System;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using MyBox;
using SceneReference = Eflatun.SceneReference.SceneReference;
using System.Collections.Generic;

namespace Systems.SceneManagment
{ 
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] SceneGroup[] sceneGroups;
        [MustBeAssigned] [SerializeField] SceneReference loadingScene;
        [MustBeAssigned] [SerializeField] LevelSettings LevelSettings;

        public readonly SceneGroupManager manager = new SceneGroupManager();

        private float targetProgress;
        private bool Init = false;

        private void Awake()
        {
            manager.OnSceneLoaded += sceneName => Debug.Log("Loaded: " + sceneName);
            manager.OnSceneUnloaded += sceneName => Debug.Log("Unloaded: " + sceneName);
            manager.OnSceneGroupLoaded += () => Debug.Log("Scene group loaded");

            ServiceLocator.Register(this);
        }

        public void OnDestroy()
        {
            ServiceLocator.Unregister<SceneLoader>();
        }

        async void Start()
        {
            if (sceneGroups[0].GroupName != "MainMenu")
                Init = true;

            await LoadSceneGroup(sceneGroups[0].GroupName);

            Init = true;
        }

        public async Task LoadSceneGroup(string groupName)
        {
            targetProgress = 1f;

            LoadingProgress progress = new LoadingProgress();
            progress.Progressed += target => targetProgress = Mathf.Max(target, targetProgress);
            

            foreach (SceneGroup group in sceneGroups)
            {
                if (group.GroupName != groupName) continue;

                SceneGroup temp = new SceneGroup();
                temp.GroupName = group.GroupName;
                temp.Scenes = new(group.Scenes);


                if (group.GroupName == "Combat")
                {
                    List<SceneData> sceneDatas = Extensions.CloneList(temp.Scenes);

                    LevelData data = GetLevelData();
                    sceneDatas.Insert(0, new SceneData(data.SceneRef, SceneType.Environment));

                    temp.Scenes = sceneDatas;
                }

                if (Init)
                    await SceneManager.LoadSceneAsync(loadingScene.Path, LoadSceneMode.Additive);

                await manager.LoadScenes(temp, progress);

                if (Init)
                    await SceneManager.UnloadSceneAsync(loadingScene.Path);

                return;
            }

            Debug.LogError($"Group Name does not exist: {groupName}");
        }

        LevelData GetLevelData()
        {
            int _currentLevel = GameManager.Instance.StageLevel;

            if (_currentLevel >= LevelSettings.Levels.Count)
                _currentLevel = LevelSettings.Levels.Count - 1;

            LevelData data = LevelSettings.Levels[_currentLevel];

            return data;

            //AvatarSpawnPosition asp = environment.GetComponent<AvatarSpawnPosition>();
            //EnemyManager.EnemySpawnPosList = asp.EnemySpawnPositionList;
            //Ctx.PlayerSpawnPos = asp.PlayerSpawnPosition;
        }
    }

    public class LoadingProgress : IProgress<float>
    {
        public event Action<float> Progressed;

        const float ratio = 1f;

        public void Report(float value)
        {
            Progressed?.Invoke(value / ratio);
        }
    }
}
