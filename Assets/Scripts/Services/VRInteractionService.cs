using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Services
{
    using UnityEngine;
    using UnityEngine.XR.Interaction.Toolkit;

    public class VRInteractionService : MonoBehaviour, IVRInteractionService
    {
        [Header("VR Settings")]
        public XRBaseInteractor leftInteractor;
        public XRBaseInteractor rightInteractor;
    
        private void Start()
        {
            ServiceLocator.Instance.RegisterService<IVRInteractionService>(this);
        }

        public void OnObjectGrabbed(GameObject obj)
        {
            Debug.Log($"Object grabbed: {obj.name}");
        
            // Notify other services
            var uiService = ServiceLocator.Instance.GetService<IUIService>();
            uiService?.ShowNotification($"Grabbed: {obj.name}");
        }

        public void OnObjectReleased(GameObject obj)
        {
            Debug.Log($"Object released: {obj.name}");
            HighlightObject(obj, false);
        }

        public void HighlightObject(GameObject obj, bool highlight)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                if (highlight)
                {
                    renderer.material.color = Color.yellow;
                }
                else
                {
                    renderer.material.color = Color.white;
                }
            }
        }
    }
}