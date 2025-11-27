using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class RaycastSystem : Singleton<RaycastSystem>
{
    public enum RaycastState { None, Running }
    [Header("Raycast Settings")]
    public RaycastState state = RaycastState.None;
    public LayerMask IgnoreLayer;
    public LayerMask OutlineLayer;
    [Header("Raycast Camera")]
    public Camera RaycastCamera;

    private readonly List<IRaycastEventsListener> listeners = new();

    void Update()
    {
        if (IsBlocking() || IsClickingUI()) return;

        if (StaticVariable.GameState == GameState.Pause)
        {
            RaycastCheckingOnPauseState();
            return;
        }
        RaycastChecking();
    }

    private void RaycastCheckingOnPauseState()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            state = RaycastState.Running;
            Ray ray = RaycastCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, ~IgnoreLayer))
            {
                foreach (var l in listeners) l.OnRaycastBeganPauseState(hit.collider.transform);
            }
        }
        if (Mouse.current.leftButton.isPressed && state == RaycastState.Running)
        {
            foreach (var l in listeners) l.OnRaycastMovePauseState(Mouse.current.position.ReadValue());
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame && state == RaycastState.Running)
        {
            state = RaycastState.None;
            foreach (var l in listeners) l.OnRaycastEndPauseState(Mouse.current.position.ReadValue());
        }
    }

    private void RaycastChecking()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            state = RaycastState.Running;
            Ray ray = RaycastCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, ~IgnoreLayer))
            {
                foreach (var l in listeners) l.OnRaycastBegan(hit);
            }
            foreach (var l in listeners) l.OnRaycastBeganPosition(Mouse.current.position.ReadValue());
        }
        if (Mouse.current.leftButton.isPressed && state == RaycastState.Running)
        {
            foreach (var l in listeners) l.OnRaycastDrag(Mouse.current.position.ReadValue());
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame && state == RaycastState.Running)
        {
            state = RaycastState.None;
            foreach (var l in listeners) l.OnRaycastEnded(Mouse.current.position.ReadValue());
        }
    }

    public bool CenterRaycast(out RaycastHit hitInfo, Vector2 mousePosition)
    {
        hitInfo = default;
        if (IsBlocking() || IsClickingUI()) return false;
        Ray ray = RaycastCamera.ScreenPointToRay(mousePosition);
        return Physics.Raycast(ray, out hitInfo, 1000f, ~IgnoreLayer);
    }

    public bool PerformRaycast(out RaycastHit hitInfo, Vector2 mousePosition)
    {
        hitInfo = default;
        if (IsBlocking() || IsClickingUI()) return false;
        Ray ray = RaycastCamera.ScreenPointToRay(mousePosition);
        return Physics.Raycast(ray, out hitInfo, 1000f, ~OutlineLayer);
    }

    public bool IsClickingUI()
    {
        // Kiểm tra khi nhấn chuột
        if (Mouse.current.leftButton.wasPressedThisFrame)
            return EventSystem.current?.currentSelectedGameObject != null;
        return false;
    }

    public bool IsBlocking() => false;

    public static void RegisterListener(IRaycastEventsListener listener)
    {
        if (!I.listeners.Contains(listener))
            I.listeners.Add(listener);
    }

    public static void UnregisterListener(IRaycastEventsListener listener)
    {
        I.listeners.Remove(listener);
    }
}
