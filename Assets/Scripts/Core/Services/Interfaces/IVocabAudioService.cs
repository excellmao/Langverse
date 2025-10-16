using UnityEngine;

namespace Core.Services.Interfaces
{
    public interface IVocabAudioService
    {
        void PlayPronunciation(string word);
        void RegisterVocabObject(GameObject obj, string word);
        void UnregisterVocabObject(GameObject obj);
        void OnObjectGrabbed(GameObject obj);
    }
}