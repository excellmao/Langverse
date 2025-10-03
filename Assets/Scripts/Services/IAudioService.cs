using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    public interface IAudioService
    {
        void PlaySound(AudioClip clip);
        void PlaySoundAtPosition(AudioClip clip, Vector3 position);
        void SetMasterVolume(float volume);
        void StopAllSounds();
    }
}