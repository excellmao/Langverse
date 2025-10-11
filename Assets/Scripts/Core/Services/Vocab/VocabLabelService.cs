using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Core.Services.Interfaces;

namespace Core.Services.Vocab
{
    public class VocabLabelService : MonoBehaviour, IVocabLabelService
    {
        private readonly Dictionary<GameObject, GameObject> labels = new();
        private Camera mainCamera;

        public bool isInitialized { get; private set; }
        public string serviceId => nameof(VocabLabelService);

        public void Init()
        {
            mainCamera = Camera.main ?? Object.FindObjectOfType<Camera>();
            isInitialized = true;
        }

        public void Cleanup()
        {
            HideAllLabels();
            isInitialized = false;
        }

        public void ShowLabel(GameObject obj, string word)
        {
            if (obj == null || string.IsNullOrEmpty(word)) return;

            if (!labels.TryGetValue(obj, out var labelObj) || labelObj == null)
            {
                labelObj = CreateLabelObject(word);
                labelObj.transform.SetParent(obj.transform);
                labelObj.transform.localPosition = Vector3.up * 1.5f;
                labels[obj] = labelObj;
            }

            labelObj.SetActive(true);
            var text = labelObj.GetComponentInChildren<TextMeshPro>();
            if (text != null) text.text = word;
        }

        public void HideLabel(GameObject obj)
        {
            if (labels.TryGetValue(obj, out var labelObj) && labelObj != null)
                labelObj.SetActive(false);
        }

        public void HideAllLabels()
        {
            foreach (var kv in labels)
                if (kv.Value != null) kv.Value.SetActive(false);
        }

        private GameObject CreateLabelObject(string word)
        {
            var labelGO = new GameObject("VocabLabel");
            var textMesh = labelGO.AddComponent<TextMeshPro>();
            textMesh.text = word;
            textMesh.fontSize = 0.3f;
            textMesh.alignment = TextAlignmentOptions.Center;
            textMesh.color = Color.yellow;
            labelGO.AddComponent<FaceCameraBehaviour>();
            return labelGO;
        }
    }

    public class FaceCameraBehaviour : MonoBehaviour
    {
        private Camera _mainCamera;
        private void Start() { _mainCamera = Camera.main; }
        private void Update()
        {
            if (_mainCamera == null) _mainCamera = Camera.main;
            if (_mainCamera == null) return;
            transform.forward = _mainCamera.transform.forward;
        }
    }
}