using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Core.Services.Interfaces;
using Core.ServiceLocator;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class VocabManager : MonoBehaviour
{
    [System.Serializable]
    public class VocabularyEntry
    {
        public GameObject interactableObject;
        public AudioClip audioClip;
        [Tooltip("Leave empty to use GameObject hierarchy name")]
        public string customDisplayName = "";
    }

    [Header("Vocabulary Settings")]
    public List<VocabularyEntry> vocabularyEntries = new List<VocabularyEntry>();

    [Header("Label Settings")]
    [SerializeField] private float textDisplayDuration = 3f;
    [SerializeField] private bool showTextOnHover = true;

    // Services
    private IAudioService _audioService;
    private IVocabLabelService _labelService;

    // Fast lookups
    private readonly Dictionary<GameObject, VocabularyEntry> _objectToEntry = new();
    private readonly Dictionary<GameObject, string> _objectToDisplayName = new();

    private void Start()
    {
        // Resolve services from ServiceLocator
        _audioService = ServiceLocator.Get<IAudioService>();
        _labelService = ServiceLocator.Get<IVocabLabelService>();

        if (_audioService == null)
        {
            Debug.LogError("[VocabManager] IAudioService not found. Make sure it is registered with ServiceLocator.");
        }
        if (_labelService == null)
        {
            Debug.LogError("[VocabManager] IVocabLabelService not found. Make sure it is registered with ServiceLocator.");
        }

        SetupVocabularyObjects();
    }

    private void SetupVocabularyObjects()
    {
        foreach (var entry in vocabularyEntries)
        {
            if (entry?.interactableObject == null) continue;

            var go = entry.interactableObject;

            // Cache entry and resolved display name
            var displayName = string.IsNullOrWhiteSpace(entry.customDisplayName)
                ? go.name
                : entry.customDisplayName;

            _objectToEntry[go] = entry;
            _objectToDisplayName[go] = displayName;

            // Hook XR events
            SetupInteractionEvents(go);

            // Prepare label via service (hidden by default)
            _labelService?.ShowLabel(go, displayName);
            _labelService?.HideLabel(go);
        }
    }

    private void SetupInteractionEvents(GameObject obj)
    {
        var grab = obj.GetComponent<XRGrabInteractable>();
        if (grab == null)
        {
            Debug.LogWarning($"[VocabManager] XRGrabInteractable not found on {obj.name}. Skipping interaction hooks.");
            return;
        }

        grab.selectEntered.AddListener(OnObjectGrabbed);
        grab.selectExited.AddListener(OnObjectReleased);

        if (showTextOnHover)
        {
            grab.hoverEntered.AddListener(OnObjectHoverEnter);
            grab.hoverExited.AddListener(OnObjectHoverExit);
        }
    }

    private void OnObjectGrabbed(SelectEnterEventArgs args)
    {
        var go = args.interactableObject.transform.gameObject;
        if (!_objectToEntry.TryGetValue(go, out var entry)) return;

        // Play pronunciation via audio service
        if (entry.audioClip != null)
        {
            _audioService?.PlayAudio(entry.audioClip, 1f);
        }

        // Show label via label service
        if (_objectToDisplayName.TryGetValue(go, out var label))
        {
            _labelService?.ShowLabel(go, label);

            // Auto-hide after duration
            if (textDisplayDuration > 0)
            {
                StartCoroutine(HideLabelAfterDelay(go, textDisplayDuration));
            }
        }
    }

    private void OnObjectReleased(SelectExitEventArgs args)
    {
        var go = args.interactableObject.transform.gameObject;
        // Optional: hide on release
        // _labelService?.HideLabel(go);
    }

    private void OnObjectHoverEnter(HoverEnterEventArgs args)
    {
        if (!showTextOnHover) return;

        var go = args.interactableObject.transform.gameObject;
        if (_objectToDisplayName.TryGetValue(go, out var label))
        {
            _labelService?.ShowLabel(go, label);
        }
    }

    private void OnObjectHoverExit(HoverExitEventArgs args)
    {
        if (!showTextOnHover) return;

        var go = args.interactableObject.transform.gameObject;
        _labelService?.HideLabel(go);
    }

    private IEnumerator HideLabelAfterDelay(GameObject targetObject, float delay)
    {
        yield return new WaitForSeconds(delay);
        _labelService?.HideLabel(targetObject);
    }

    // Public helpers
    public void ShowAllLabels()
    {
        foreach (var kv in _objectToDisplayName)
        {
            _labelService?.ShowLabel(kv.Key, kv.Value);
        }
    }

    public void HideAllLabels()
    {
        foreach (var kv in _objectToDisplayName)
        {
            _labelService?.HideLabel(kv.Key);
        }
    }

    public void UpdateObjectDisplayName(GameObject obj, string newName)
    {
        if (!_objectToDisplayName.ContainsKey(obj)) return;

        _objectToDisplayName[obj] = newName;
        _labelService?.ShowLabel(obj, newName);
    }
}