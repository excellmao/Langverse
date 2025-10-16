using Core.ServiceLocator;
using UnityEngine;

namespace Core.Services.Interfaces
{
    public interface IVocabLabelService: IService
    {
        void ShowLabel(GameObject obj, string word);
        void HideLabel(GameObject obj);
        void HideAllLabels();
    }
}