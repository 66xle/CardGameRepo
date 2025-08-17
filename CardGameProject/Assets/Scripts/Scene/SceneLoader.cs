using UnityEngine;
using System;
using System.Threading.Tasks;

namespace Systems.SceneManagment
{ 
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] SceneGroup[] sceneGroups;

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
