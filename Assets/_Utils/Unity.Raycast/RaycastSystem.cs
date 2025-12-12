using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class RaycastSystem : Singleton<RaycastSystem>
{
    public enum RaycastState { None, Running }

    [Header("Raycast Settings")]
    public RaycastState CurrentRaycastState = RaycastState.None;
    public LayerMask IgnoreRaycastLayer;
    public LayerMask OutlineRaycastLayer;

    [Header("Raycast Camera")]
    public Camera RaycastCamera;

    private readonly List<IRaycastEventsListener> _eventListeners = new();

    void Update()
    {
        if (IsRaycastBlocked() || IsPointerOverUI()) return;
        HandleRaycast();
    }

    private void HandleRaycastWhilePaused()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            CurrentRaycastState = RaycastState.Running;
            Ray ray = RaycastCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, ~IgnoreRaycastLayer))
            {
                foreach (var listener in _eventListeners)
                    listener.OnRaycastBeganPauseState(hit.collider.transform);
            }
        }
        if (Mouse.current.leftButton.isPressed && CurrentRaycastState == RaycastState.Running)
        {
            foreach (var listener in _eventListeners)
                listener.OnRaycastMovePauseState(Mouse.current.position.ReadValue());
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame && CurrentRaycastState == RaycastState.Running)
        {
            CurrentRaycastState = RaycastState.None;
            foreach (var listener in _eventListeners)
                listener.OnRaycastEndPauseState(Mouse.current.position.ReadValue());
        }
    }

    private void HandleRaycast()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            CurrentRaycastState = RaycastState.Running;
            Ray ray = RaycastCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, ~IgnoreRaycastLayer))
            {
                foreach (var listener in _eventListeners)
                    listener.OnRaycastBegan(hit);
            }
            foreach (var listener in _eventListeners)
                listener.OnRaycastBeganPosition(Mouse.current.position.ReadValue());
        }
        if (Mouse.current.leftButton.isPressed && CurrentRaycastState == RaycastState.Running)
        {
            foreach (var listener in _eventListeners)
                listener.OnRaycastDrag(Mouse.current.position.ReadValue());
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame && CurrentRaycastState == RaycastState.Running)
        {
            CurrentRaycastState = RaycastState.None;
            foreach (var listener in _eventListeners)
                listener.OnRaycastEnded(Mouse.current.position.ReadValue());
        }
    }

    public bool CenterRaycast(out RaycastHit hitInfo, Vector2 screenPosition)
    {
        hitInfo = default;
        if (IsRaycastBlocked() || IsPointerOverUI()) return false;
        Ray ray = RaycastCamera.ScreenPointToRay(screenPosition);
        return Physics.Raycast(ray, out hitInfo, 1000f, ~IgnoreRaycastLayer);
    }

    public bool PerformRaycast(out RaycastHit hitInfo, Vector2 screenPosition)
    {
        hitInfo = default;
        if (IsRaycastBlocked() || IsPointerOverUI()) return false;
        Ray ray = RaycastCamera.ScreenPointToRay(screenPosition);
        return Physics.Raycast(ray, out hitInfo, 1000f, ~OutlineRaycastLayer);
    }

    public bool IsPointerOverUI()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
            return EventSystem.current?.currentSelectedGameObject != null;
        return false;
    }

    public bool IsRaycastBlocked() => false;

    public static void RegisterListener(IRaycastEventsListener listener)
    {
        if (!I._eventListeners.Contains(listener))
            I._eventListeners.Add(listener);
    }

    public static void UnregisterListener(IRaycastEventsListener listener)
    {
        I._eventListeners.Remove(listener);
    }
}
