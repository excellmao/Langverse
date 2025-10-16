<<<<<<< HEAD
﻿using UnityEngine;

namespace Core.Services.Interfaces
{
    public interface IVocabAudioService
    {
        void PlayPronunciation(string word);
        void RegisterVocabObject(GameObject obj, string word);
        void UnregisterVocabObject(GameObject obj);
=======
﻿using Core.ServiceLocator;
using UnityEngine;

namespace Core.Services.Interfaces
{
    public interface IVocabularyAudioService : IService
    {
        void PlayPronunciation(string word);
        void RegisterVocabularyObject(GameObject obj, string word);
        void UnregisterVocabularyObject(GameObject obj);
>>>>>>> 5e82e6c1693de831bca29880ee0a56390795bfd7
        void OnObjectGrabbed(GameObject obj);
    }
}