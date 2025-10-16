using UnityEngine;
using Core.ServiceLocator;

namespace Core.Services.Interfaces
{
    // Resolved merge conflict and unified with VocabManager usage
    public interface IVocabAudioService : IService
    {
        void PlayPronunciation(string word);
        void RegisterVocabularyObject(GameObject obj, string word);
        void UnregisterVocabularyObject(GameObject obj);
        void OnObjectGrabbed(GameObject obj);
    }
}