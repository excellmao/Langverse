using Core.ServiceLocator;
using UnityEngine;

namespace Core.Interfaces
{
    public interface IVocabAudioService: IService
    {
        void PlayPronunciation(string word);
        void RegisterVocabObject(GameObject obj, string word);
        void UnregisterVocabObject(GameObject obj);
        void OnObjectGrabbed(GameObject obj);
    }
}