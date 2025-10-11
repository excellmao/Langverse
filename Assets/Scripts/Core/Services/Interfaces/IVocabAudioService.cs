using Core.ServiceLocator;
using UnityEngine;

namespace Core.Services.Interfaces
{
    public interface IVocabularyAudioService : IService
    {
        void PlayPronunciation(string word);
        void RegisterVocabularyObject(GameObject obj, string word);
        void UnregisterVocabularyObject(GameObject obj);
        void OnObjectGrabbed(GameObject obj);
    }
}