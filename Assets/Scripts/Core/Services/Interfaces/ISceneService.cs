using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using Core.ServiceLocator;

namespace Core.Services.Interfaces
{
    public interface ISceneService : IService
    {
        // Events
        event Action<string> OnSceneLoadStarted;
        event Action<string> OnSceneLoadCompleted;
        event Action<string, float> OnSceneLoadProgress;
        event Action<string> OnSceneUnloaded;
        
        // Transition
        void LoadScene(string sceneName);
        void LoadSceneAsync(string sceneName, Action onComplete = null);
        void UnloadScene(string sceneName);
        Coroutine LoadSceneWithFade(string sceneName, float fadeDuration = 1f);
        
        // State
        bool IsSceneLoaded(string sceneName);
        string GetCurrentSceneName();
        Scene GetCurrentScene();
        float GetSceneLoadProgress();
        
        // VR-Specific
        void ShowVRLoadingScreen(string message = "Loading...");
        void HideVRLoadingScreen();
        void UpdateLoadingProgress(float progress, string statusText = "");
        
        // Comfort
        void EnableFadeTransition(bool enabled);
        void SetFadeColor(Color fadeColor);
        void SetFadeDuration(float duration);
        
        // Player Position
        void SavePlayerPosition();
        void RestorePlayerPosition();
        void SetPlayerSpawnPoint(Vector3 position, Quaternion rotation);
        Vector3 GetPlayerSpawnPoint();
        
        // Scene Validation
        bool IsSceneVRCompatible(string sceneName);
        void ValidateSceneForVR(Scene scene);
        
        // Loading Screen Customization
        void SetLoadingScreenPrefab(GameObject prefab);
        void SetLoadingScreenPosition(Vector3 position);
        void SetLoadingScreenRotation(Quaternion rotation);
        
        // Scene Preloading
        void PreloadScene(string sceneName);
        void UnloadPreloadedScene(string sceneName);
        bool IsScenePreloaded(string sceneName);
        
        // Multi-Scene Management
        void LoadAdditiveScene(string sceneName);
        void UnloadAdditiveScene(string sceneName);
        Scene[] GetAllLoadedScenes();
    }
}