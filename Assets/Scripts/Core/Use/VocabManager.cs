using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using Core.ServiceLocator;
using Core.Interfaces;
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

    [Header("Text Display Settings")]
    [SerializeField] private float textSize = 0.3f;
    [SerializeField] private Color textColor = Color.white;
    [SerializeField] private Vector3 textOffset = new Vector3(0, 1.5f, 0);
    [SerializeField] private float textDisplayDuration = 3f;
    [SerializeField] private bool showTextOnHover = true;

    private IVocabAudioService audioService;
    private IVocabLabelService labelService;
    private readonly Dictionary<GameObject, string> objectToWord = new();
    private readonly Dictionary<GameObject, string> objectToCustomName = new();

    private void Start()
    {
        audioService = ServiceLocator.Get<IVocabAudioService>();
        labelService = ServiceLocator.Get<IVocabLabelService>();

        if (audioService == null)
        {
            Debug.LogError("[VocabManager] IVocabularyAudioService not found. Make sure it is registered with ServiceLocator.");
        }
        if (labelService == null)
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

            var displayName = string.IsNullOrEmpty(entry.customDisplayName)
                ? entry.interactableObject.name
                : entry.customDisplayName;

            // Register with audio service
            audioService?.RegisterVocabObject(entry.interactableObject, displayName);

            // Register with label service (hidden by default)
            labelService?.ShowLabel(entry.interactableObject, displayName);
            labelService?.HideLabel(entry.interactableObject);

            objectToWord[entry.interactableObject] = displayName;
            objectToCustomName[entry.interactableObject] = entry.customDisplayName;

            SetupInteractionEvents(entry);
        }
    }

    private void SetupInteractionEvents(VocabularyEntry entry)
    {
        var grabInteractable = entry.interactableObject.GetComponent<XRGrabInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnObjectGrabbed);
            grabInteractable.selectExited.AddListener(OnObjectReleased);

            if (showTextOnHover)
            {
                grabInteractable.hoverEntered.AddListener(OnObjectHoverEnter);
                grabInteractable.hoverExited.AddListener(OnObjectHoverExit);
            }
        }
    }

    private void OnObjectGrabbed(SelectEnterEventArgs args)
    {
        var grabbedObject = args.interactableObject.transform.gameObject;

        if (objectToWord.TryGetValue(grabbedObject, out var vocabWord))
        {
            audioService?.PlayPronunciation(vocabWord);
            labelService?.ShowLabel(grabbedObject, vocabWord);

            if (textDisplayDuration > 0)
                StartCoroutine(HideLabelAfterDelay(grabbedObject, textDisplayDuration));
        }

        audioService?.OnObjectGrabbed(grabbedObject);
    }

    private void OnObjectReleased(SelectExitEventArgs args)
    {
        var releasedObject = args.interactableObject.transform.gameObject;
        labelService?.HideLabel(releasedObject);
    }

    private void OnObjectHoverEnter(HoverEnterEventArgs args)
    {
        var obj = args.interactableObject.transform.gameObject;
        if (objectToWord.TryGetValue(obj, out var vocabWord))
        {
            labelService?.ShowLabel(obj, vocabWord);
        }
    }

    private void OnObjectHoverExit(HoverExitEventArgs args)
    {
        var obj = args.interactableObject.transform.gameObject;
        labelService?.HideLabel(obj);
    }

    private System.Collections.IEnumerator HideLabelAfterDelay(GameObject obj, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        labelService?.HideLabel(obj);
    }
}