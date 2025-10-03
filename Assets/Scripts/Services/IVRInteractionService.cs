using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Services
{
    public interface IVRInteractionService
    {
        void OnObjectGrabbed(GameObject obj);
        void OnObjectReleased(GameObject obj);
        void HighlightObject(GameObject obj, bool highlight);
        void SetHapticFeedback(float intensity, float duration);
    }
}