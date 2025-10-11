using System.Collections.Generic;
using UnityEngine;
using Core.Services.Interfaces;

namespace Core.Services.Vocab
{
    public class VocabularyAudioService : IVocabularyAudioService
    {
        private readonly Dictionary<GameObject, string> objectToWord = new();
        private readonly Dictionary<string, AudioClip> wordToClip = new();
        private AudioSource audioSource;

        public bool isInitialized { get; private set; }
        public string serviceId => nameof(VocabularyAudioService);

        public void Init()
        {
            audioSource = Camera.main?.GetComponent<AudioSource>();
            if (audioSource == null && Camera.main != null)
                audioSource = Camera.main.gameObject.AddComponent<AudioSource>();
            isInitialized = true;
        }

        public void Cleanup() { objectToWord.Clear(); wordToClip.Clear(); isInitialized = false; }
        public void RegisterVocabularyObject(GameObject obj, string word)
        {
            if (obj != null && !string.IsNullOrEmpty(word))
                objectToWord[obj] = word;
        }
        public void UnregisterVocabularyObject(GameObject obj) { if (obj != null) objectToWord.Remove(obj); }
        public void PlayPronunciation(string word)
        {
            if (wordToClip.TryGetValue(word, out var clip) && audioSource != null)
            {
                audioSource.Stop();
                audioSource.clip = clip;
                audioSource.Play();
            }
        }
        public void OnObjectGrabbed(GameObject obj)
        {
            if (objectToWord.TryGetValue(obj, out var word))
                PlayPronunciation(word);
        }
    }
}
