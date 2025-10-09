using UnityEngine;
using UnityEngine.XR;
using System;

namespace Core.Services.Interfaces
{
    public interface IVRInputService : IVRService
    {
        // Controller Input
        event Action<XRNode, bool> OnTriggerPressed;
        event Action<XRNode, bool> OnGripPressed;
        event Action<XRNode, Vector2> OnTouchpadInput;
        event Action<XRNode, Vector2> OnJoystickInput;
        event Action<XRNode, bool> OnMenuButtonPressed;
        
        // Hand Tracking
        event Action<XRNode, Vector3> OnHandPositionChanged;
        event Action<XRNode, Quaternion> OnHandRotationChanged;
        event Action<XRNode, float> OnHandGripStrength;
        bool IsHandTrackingEnabled { get; }
        bool IsHandTracked(XRNode node);
        Vector3 GetHandPosition(XRNode node);
        Quaternion GetHandRotation(XRNode node);
        float GetHandConfidence(XRNode node);
        
        // Gesture Recognition
        event Action<XRNode, string> OnGestureRecognized;
        void EnableGestureRecognition(bool enable);
        void RegisterCustomGesture(string gestureName, Func<XRNode, bool> gestureDetector);
        void UnregisterCustomGesture(string gestureName);
        
        // Controller State
        bool IsControllerConnected(XRNode node);
        Vector3 GetControllerPosition(XRNode node);
        Quaternion GetControllerRotation(XRNode node);
        Vector3 GetControllerVelocity(XRNode node);
        Vector3 GetControllerAngularVelocity(XRNode node);
        
        // Input State
        bool GetTriggerPressed(XRNode node);
        float GetTriggerValue(XRNode node);
        bool GetGripPressed(XRNode node);
        float GetGripValue(XRNode node);
        Vector2 GetTouchpadValue(XRNode node);
        Vector2 GetJoystickValue(XRNode node);
        bool GetMenuButtonPressed(XRNode node);
        
        // Haptic Feedback
        void TriggerHapticFeedback(XRNode node, float amplitude, float duration);
        void TriggerHapticPattern(XRNode node, float[] pattern, float intensity);
    }
}