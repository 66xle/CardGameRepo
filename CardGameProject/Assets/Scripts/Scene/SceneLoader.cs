using UnityEngine;
using System;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using MyBox;
using SceneReference = Eflatun.SceneReference.SceneReference;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using PixelCrushers;

namespace Systems.SceneManagment
{ 
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] SceneGroup[] sceneGroups;
        [MustBeAssigned] [SerializeField] SceneReference loadingScene;
        [MustBeAssigned] [SerializeField] GameObject LoadingScreen;
        [MustBeAssigned] [SerializeField] Image FadeImage;
        [MustBeAssigned] [SerializeField] float FadeTime = 1f;
        [MustBeAssigned] [SerializeField] LevelSettings LevelSettings;

        public readonly SceneGroupManager manager = new SceneGroupManager();

        private float targetProgress;
        private bool Init = false;

        private bool isLoadingFinished = false;

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

        public void CloseLoadingScreen()
        {
            isLoadingFinished = true;
        }

        public async Task LoadSceneGroup(string groupName)
        {
            targetProgress = 1f;
            isLoadingFinished = false;

            LoadingProgress progress = new LoadingProgress();
            progress.Progressed += target => targetProgress = Mathf.Max(target, targetProgress);
            

            foreach (SceneGroup group in sceneGroups)
            {
                if (group.GroupName != groupName) continue;

                SceneGroup temp = new SceneGroup();
                temp.GroupName = group.GroupName;
                temp.Scenes = new(group.Scenes);

                if (Init)
                {
                    await DOVirtual.Float(FadeImage.color.a, 1f, FadeTime, a => FadeImage.SetAlpha(a)).AsyncWaitForCompletion();

                    //LoadingScreen.SetActive(true);
                    await SceneManager.LoadSceneAsync(loadingScene.Path, LoadSceneMode.Additive);
                    await manager.UnloadScenes();

                    await DOVirtual.Float(FadeImage.color.a, 0f, FadeTime, a => FadeImage.SetAlpha(a)).AsyncWaitForCompletion();
                }

                if (group.GroupName == "Combat")
                {
                    if (GameManager.Instance.LoadedEnvironment != null)
                        Destroy(GameManager.Instance.LoadedEnvironment);

                    LevelData data = GetLevelData();
                    GameManager.Instance.CurrentLevelDataLoaded = data;
                    GameObject environment = Instantiate(data.Prefab);
                    GameManager.Instance.LoadedEnvironment = environment;
                }

                await manager.LoadScenes(temp, progress);

                //while (!isLoadingFinished)
                //{
                //    await Task.Yield();
                //}

                if (Init)
                {
                    //await DOVirtual.Float(FadeImage.color.a, 1f, FadeTime, a => FadeImage.SetAlpha(a)).AsyncWaitForCompletion();

                    //LoadingScreen.SetActive(false);
                    //await SceneManager.UnloadSceneAsync(loadingScene.Path);

                    //await DOVirtual.Float(FadeImage.color.a, 0f, FadeTime, a => FadeImage.SetAlpha(a)).AsyncWaitForCompletion();
                }

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
