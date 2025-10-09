using UnityEngine;
using System;
using Core.ServiceLocator;

namespace Core.Services.Interfaces
{
    [System.Serializable]
    public class VRComfortSettings
    {
        public bool vignetteEnabled = true;
        public bool snapTurnEnabled = true;
        public float snapTurnAngle = 30f;
        public bool teleportFadeEnabled = true;
        public float movementSpeed = 3f;
        public bool smoothTurnEnabled = false;
        public float smoothTurnSpeed = 90f;
    }
    
    [System.Serializable]
    public class VRGraphicsSettings
    {
        public int renderScale = 100;
        public bool foveatedRenderingEnabled = false;
        public int msaaLevel = 4;
        public bool dynamicResolutionEnabled = true;
        public int targetFrameRate = 90;
    }
    
    [System.Serializable]
    public class VRAudioSettings
    {
        public float masterVolume = 1f;
        public float sfxVolume = 1f;
        public float musicVolume = 0.7f;
        public float voiceVolume = 1f;
        public bool spatialAudioEnabled = true;
        public bool voiceChatEnabled = true;
    }

    public interface ISettingsService : IService
    {
        // Events
        event Action OnSettingsChanged;
        event Action<VRComfortSettings> OnComfortSettingsChanged;
        event Action<VRGraphicsSettings> OnGraphicsSettingsChanged;
        event Action<VRAudioSettings> OnAudioSettingsChanged;
        
        // Comfort Settings
        VRComfortSettings GetComfortSettings();
        void SetComfortSettings(VRComfortSettings settings);
        void ApplyComfortSettings();
        
        // Graphics Settings
        VRGraphicsSettings GetGraphicsSettings();
        void SetGraphicsSettings(VRGraphicsSettings settings);
        void ApplyGraphicsSettings();
        
        // Audio Settings
        VRAudioSettings GetAudioSettings();
        void SetAudioSettings(VRAudioSettings settings);
        void ApplyAudioSettings();
        
        // Individual Setting Methods
        void SetRenderScale(int scale);
        void SetMSAALevel(int level);
        void SetTargetFrameRate(int frameRate);
        void SetMovementSpeed(float speed);
        void SetSnapTurnAngle(float angle);
        void SetMasterVolume(float volume);
        
        // Accessibility
        void SetColorBlindnessSupport(bool enabled);
        void SetFontSize(float scale);
        void SetHighContrastMode(bool enabled);
        void SetSubtitlesEnabled(bool enabled);
        
        // Performance Settings
        void SetPerformanceMode(string mode); // "Quality", "Balanced", "Performance"
        string GetCurrentPerformanceMode();
        void SetDynamicResolution(bool enabled);
        void SetFoveatedRendering(bool enabled);
        
        // Persistence
        void SaveSettings();
        void LoadSettings();
        void ResetToDefaults();
        
        // Developer Settings
        void EnableDeveloperMode(bool enabled);
        bool IsDeveloperModeEnabled();
        void SetShowFPS(bool show);
        void SetShowPerformanceStats(bool show);
    }
}