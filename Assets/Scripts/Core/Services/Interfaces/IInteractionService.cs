using UnityEngine;
using System;
using Core.ServiceLocator;

namespace Core.Services.Interfaces
{
    public interface IInteractionService : IService
    {
        //Events
        event Action<GameObject, Transform> OnObjectGrabbed;
        event Action<GameObject, Transform> OnObjectReleased;
        event Action<GameObject> OnObjectHovered;
        event Action<GameObject> OnObjectUnhovered;
        event Action<GameObject, Transform> OnObjectUsed;
        
        //Grabbing
        bool CanGrabObject(GameObject obj, Transform hand);
        void GrabObject(GameObject obj, Transform hand);
        void ReleaseObject(Transform hand);
        GameObject GetGrabbedObject(Transform hand);
        bool IsHandGrabbingObject(Transform hand);
        
        // Distance Interaction
        void EnableDistanceGrabbing(float maxDistance = 10f);
        void DisableDistanceGrabbing();
        bool TryDistanceGrab(Transform hand, out GameObject grabbedObject);
        
        // Throwing
        void SetThrowForce(float force);
        void SetThrowAngularForce(float angularForce);
        Vector3 CalculateThrowVelocity(Transform hand);
        void ThrowObject(GameObject obj, Vector3 velocity, Vector3 angularVelocity);
        
        // Highlighting
        void HighlightObject(GameObject obj, Color highlightColor);
        void RemoveHighlight(GameObject obj);
        void SetDefaultHighlightColor(Color color);
        
        // Interaction Zones
        void CreateInteractionZone(Vector3 center, float radius, Action<Transform> onEnter, Action<Transform> onExit);
        void RemoveInteractionZone(Vector3 center);
        
        // Haptic Feedback
        void PlayGrabHaptics(Transform hand);
        void PlayReleaseHaptics(Transform hand);
        void PlayInteractionHaptics(Transform hand, float intensity = 0.5f);
        
        // Object Physics
        void SetObjectKinematic(GameObject obj, bool kinematic);
        void SetObjectGravity(GameObject obj, bool useGravity);
        void FreezeObjectRotation(GameObject obj, bool freeze);
        
        // Interaction Settings
        void SetGrabRadius(float radius);
        void SetInteractionLayers(LayerMask layers);
        LayerMask GetInteractionLayers();
        void SetHapticFeedbackEnabled(bool enabled);
    }
}