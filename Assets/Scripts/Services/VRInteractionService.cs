using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace Services
{
    using UnityEngine;
    using UnityEngine.XR.Interaction.Toolkit;

    public class VRInteractionService : MonoBehaviour, IVRInteractionService
    {
        [Header("VR Settings")]
        public XRBaseController leftController;
        public XRBaseController rightController;
    
        private void Start()
        {
            ServiceLocator.Instance.RegisterService<IVRInteractionService>(this);
        }

        public void OnObjectGrabbed(GameObject obj)
        {
            Debug.Log($"Object grabbed: {obj.name}");
            SetHapticFeedback(0.5f, 0.1f);
        
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

        public void SetHapticFeedback(float intensity, float duration)
        {
            if (leftController != null)
            {
                leftController.SendHapticImpulse(intensity, duration);
            }
            if (rightController != null)
            {
                rightController.SendHapticImpulse(intensity, duration);
            }
        }
    }
}