using UnityEngine;
using System;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using MyBox;
using SceneReference = Eflatun.SceneReference.SceneReference;

namespace Systems.SceneManagment
{ 
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] SceneGroup[] sceneGroups;
        [MustBeAssigned][SerializeField] SceneReference loadingScene;

        public readonly SceneGroupManager manager = new SceneGroupManager();

        float targetProgress;

        private void Awake()
        {
            manager.OnSceneLoaded += sceneName => Debug.Log("Loaded: " + sceneName);
            manager.OnSceneUnloaded += sceneName => Debug.Log("Unloaded: " + sceneName);
            manager.OnSceneGroupLoaded += () => Debug.Log("Scene group loaded");
        }

        async void Start()
        {
            await LoadSceneGroup(0);
        }

        public async Task LoadSceneGroup(int index)
        {
            targetProgress = 1f;

            if (index < 0 || index >= sceneGroups.Length)
            { 
                Debug.LogError("Invalid scene group index: " + index);
                return;
            }

            LoadingProgress progress = new LoadingProgress();
            progress.Progressed += target => targetProgress = Mathf.Max(target, targetProgress);

            
            await manager.LoadScenes(sceneGroups[index], progress);
            await SceneManager.LoadSceneAsync(loadingScene.Path, LoadSceneMode.Additive);
        }

        [ButtonMethod]
        public async void LoadCombat()
        {
            await LoadSceneGroup(1);
        }

        [ButtonMethod]
        public async void LoadMainMenu()
        {
            await LoadSceneGroup(0);
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
