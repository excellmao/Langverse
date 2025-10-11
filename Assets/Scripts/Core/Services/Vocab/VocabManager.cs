using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using Core.ServiceLocator;
using Core.Services.Interfaces;
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

    private IVocabularyAudioService audioService;
    private IVocabLabelService labelService;
    private Dictionary<GameObject, string> objectToWord = new();
    private Dictionary<GameObject, string> objectToCustomName = new();

    private void Start()
    {
        // Get services from the ServiceLocator
        audioService = ServiceLocator.Get<IVocabularyAudioService>();
        labelService = ServiceLocator.Get<IVocabLabelService>();

        SetupVocabularyObjects();
    }

    private void SetupVocabularyObjects()
    {
        foreach (var entry in vocabularyEntries)
        {
            if (entry.interactableObject != null)
            {
                // Register with audio service
                audioService?.RegisterVocabularyObject(entry.interactableObject, entry.customDisplayName == "" ? entry.interactableObject.name : entry.customDisplayName);

                // Register with label service
                labelService?.ShowLabel(entry.interactableObject, entry.customDisplayName == "" ? entry.interactableObject.name : entry.customDisplayName);
                labelService?.HideLabel(entry.interactableObject); // Hide by default

                objectToWord[entry.interactableObject] = entry.customDisplayName == "" ? entry.interactableObject.name : entry.customDisplayName;
                objectToCustomName[entry.interactableObject] = entry.customDisplayName;

                SetupInteractionEvents(entry);
            }
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
        GameObject grabbedObject = args.interactableObject.transform.gameObject;

        if (objectToWord.TryGetValue(grabbedObject, out string vocabWord))
        {
            // Play audio
            audioService?.PlayPronunciation(vocabWord);

            // Show label
            labelService?.ShowLabel(grabbedObject, vocabWord);

            // Auto-hide label after duration
            if (textDisplayDuration > 0)
                StartCoroutine(HideLabelAfterDelay(grabbedObject, textDisplayDuration));
        }
    }

    private void OnObjectReleased(SelectExitEventArgs args)
    {
        GameObject releasedObject = args.interactableObject.transform.gameObject;
        // Optionally hide label on release
        // labelService?.HideLabel(releasedObject);
    }

    private void OnObjectHoverEnter(HoverEnterEventArgs args)
    {
        if (showTextOnHover)
        {
            GameObject hoveredObject = args.interactableObject.transform.gameObject;
            if (objectToWord.TryGetValue(hoveredObject, out string vocabWord))
                labelService?.ShowLabel(hoveredObject, vocabWord);
        }
    }

    private void OnObjectHoverExit(HoverExitEventArgs args)
    {
        if (showTextOnHover)
        {
            GameObject hoveredObject = args.interactableObject.transform.gameObject;
            labelService?.HideLabel(hoveredObject);
        }
    }

    private System.Collections.IEnumerator HideLabelAfterDelay(GameObject targetObject, float delay)
    {
        yield return new WaitForSeconds(delay);
        labelService?.HideLabel(targetObject);
    }

    // Public methods for external control
    public void ShowAllLabels()
    {
        foreach (var kvp in objectToWord)
            labelService?.ShowLabel(kvp.Key, kvp.Value);
    }

    public void HideAllLabels()
    {
        foreach (var kvp in objectToWord)
            labelService?.HideLabel(kvp.Key);
    }

    public void UpdateObjectDisplayName(GameObject obj, string newName)
    {
        if (objectToWord.ContainsKey(obj))
        {
            objectToWord[obj] = newName;
            labelService?.ShowLabel(obj, newName);
        }
    }
}