using UnityEngine;
using Core.Services.Interfaces;

public interface IAudioService : IVRService
{
    void PlayAudio(AudioClip clip, float volume = 1f, bool loop = false);
    void PlayOneShot(AudioClip clip, float volume = 1f);
    void StopAudio();
    void PauseAudio();
    void ResumeAudio();
    void SetVolume(float volume);
    float GetVolume();
    bool IsPlaying { get; }
}