using UnityEngine;
using System;

namespace Core.Services.Interfaces
{
    public interface IAudioService: IVRService
    {
        //Events
        event Action<AudioClip> AudioClipPlayed;
        
        //Methods
        void PlayAudio(AudioClip clip, Vector3 position, float volume = 1f);
        void PlayAudio(AudioClip clip, Transform source, float volume = 1f);
        void StopAudio(AudioClip clip);
        void StopAllAudio();
        
        //Audio Sources
        AudioSource CreateAudioSource(Vector3 position);
        AudioSource CreateAudioSource(Transform source);
        void DestroyAudioSource(AudioSource source);
        
        //Volume
        void SetMasterVolume(float volume);
        float GetMasterVolume();
        void SetSfxVolume(float volume);
        float GetSfxVolume();
        void SetMusicVolume(float volume);
        float GetMusicVolume();
    }
}