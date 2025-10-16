using UnityEngine;
using System;
using Core.ServiceLocator;

namespace Core.Services.Interfaces
{
    public enum LocomotionType
    {
        Teleportation,
        SmoothMovement,
        Hybrid
    }

    public enum ComfortSetting
    {
        Comfortable,
        Moderate,
        Intense
    }
    public interface IMovementService: IService
    {
        //Events
        event Action<Vector3> OnTeleport;
        event Action<Vector3> OnPlayerMoved;
        event Action<float> OnPlayerRotated;
        
        //Teleport
        void Teleport(Vector3 destination);
        void TeleportWithRotation(Vector3 destination, float yRotation);
        bool IsTeleportValid(Vector3 destination);
        void SetTeleportRange(float maxRange);
        void ShowTeleportPreview(Vector3 position, bool isValid);
        void HideTeleportPreview();
        
        //Smooth
        void SetMovementSpeed(float speed);
        void SetRotationSpeed(float speed);
        float GetMovementSpeed();
        float GetRotationSpeed();
        
        //Snap Turn
        void EnableSnapTurn(float snapAngle = 30f);
        void DisableSnapTurn();
        void SnapTurn(float angle);
        
        //Comfort Settings
        void SetComfortSettings(ComfortSetting comfortSetting);
        ComfortSetting GetComfortSettings();
        void SetVigneteEnabled(bool enabled);
        void SetFadeOnTarget(bool enabled);
        
        //Player Bounds
        void SetPlayArena(Bounds bounds);
        Bounds GetPlayArena();
        bool IsPlayerInsideBounds();
        void ShowBoundsWarning(bool show);
        
        //Collision
        void SetCollisionEnabled(bool enabled);
        bool IsCollisionEnabled();
        LayerMask GetCollisionLayers();
        void SetCollisionLayers(LayerMask layers);
        
        //Height adjustment
        void CalibrateHeight(float height);
        void SetPlayerHeight(float height);
        float GetPlayerHeight();
        
        //Validate
        bool CanMoveToPosition(Vector3 position);
        Vector3 GetValidatedPosition(Vector3 desiredPosition);
    }
}