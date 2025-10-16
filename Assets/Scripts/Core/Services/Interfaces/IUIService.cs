using UnityEngine;
using UnityEngine.UI;
using System;
using Core.Services.Interfaces;
using UnityEngine.XR;

namespace Core.Services.Interfaces
{

    public interface IUIService : IVRService
    {
        // Events
        event Action<Canvas> CanvasCreated;
        event Action<Canvas> CanvasDestroyed;
        event Action<Button> ButtonInteracted;
        event Action<Vector3> UIRaycastHit;
        
        // Manage
        Canvas CreateWorldSpaceCanvas(Vector3 position, Vector3 rotation, Vector2 size);
        Canvas CreateWorldSpaceCanvas(Transform parent, Vector2 size);
        void DestroyCanvas(Canvas canvas);
        void SetCanvasDistance(Canvas canvas, float distance);
        void SetCanvasScale(Canvas canvas, float scale);
        
        // Interaction
        void EnableUIInteraction(bool enable);
        bool IsUIInteractionEnabled { get; }
        void SetUIRaycastDistance(float distance);
        void SetUIRaycastLayers(LayerMask layers);
        
        // Virtual Keyboard
        void ShowVirtualKeyboard(InputField targetField);
        void HideVirtualKeyboard();
        bool IsVirtualKeyboardVisible { get; }
        
        // Pointer
        void SetUIPointerVisibility(bool visible);
        void SetUIPointerColor(Color color);
        void SetUIPointerLength(float length);
        GameObject GetUIPointer(XRNode node);
        
        // Menu Systems
        GameObject CreateRadialMenu(Vector3 position, string[] menuItems);
        GameObject CreateFloatingPanel(Vector3 position, Vector2 size);
        void ShowContextMenu(Vector3 position, string[] options, Action<int> onSelected);
        void HideContextMenu();
        
        // Animation
        void AnimateCanvasIn(Canvas canvas, float duration = 0.3f);
        void AnimateCanvasOut(Canvas canvas, float duration = 0.3f, Action onComplete = null);
        void FadeCanvas(Canvas canvas, float targetAlpha, float duration);
        
        
        // Templates
        GameObject CreateButtonFromTemplate(string templateName, Transform parent);
        GameObject CreatePanelFromTemplate(string templateName, Transform parent);
        void RegisterUITemplate(string templateName, GameObject template);
        void UnregisterUITemplate(string templateName);
    }
}