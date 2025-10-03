using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    public class AudioService : MonoBehaviour, IAudioService
    {
        [Header ("Audio Settings")]
        public AudioSource masterAudioSource;
        public float masterVolume = 1f;

        private void Start()
        {
            ServiceLocator.Instance.RegisterService<IAudioService>(this);

            if (masterAudioSource == null)
            {
                masterAudioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        public void PlaySound(AudioClip clip)
        {
            if (clip != null && masterAudioSource != null)
            {
                masterAudioSource.PlayOneShot(clip, masterVolume);
            }
        }

        public void PlaySoundAtPosition(AudioClip clip, Vector3 position)
        {
            if (clip != null)
            {
                AudioSource.PlayClipAtPoint(clip, position, masterVolume);
            }
        }

        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            if (masterAudioSource != null)
            {
                masterAudioSource.volume = masterVolume;
            }
        }
        
        public void StopAllSounds()
        {
            if (masterAudioSource != null)
            {
                masterAudioSource.Stop();
            }
        }
    }
}