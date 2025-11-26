using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class RaycastSystem : MonoSingleton<RaycastSystem>
{
    public enum RaycastState { None, Running }
    public RaycastState state = RaycastState.None;
    public LayerMask IgnoreLayer;
    public LayerMask OutlineLayer;
    [Header("Raycast Camera")]
    public Camera RaycastCamera;

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
                GameEvent.OnTouchBeganPauseStateMethod(hit.collider.transform);
        }
        if (Mouse.current.leftButton.isPressed && state == RaycastState.Running)
            GameEvent.OnTouchMovePauseStateMethod(Mouse.current.position.ReadValue());

        if (Mouse.current.leftButton.wasReleasedThisFrame && state == RaycastState.Running)
        {
            state = RaycastState.None;
            GameEvent.OnTouchEndPauseStateMethod(Mouse.current.position.ReadValue());
        }
    }

    private void RaycastChecking()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            state = RaycastState.Running;
            GameEvent.OnTouchBeganMethod(Mouse.current.position.ReadValue());
            Ray ray = RaycastCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, ~IgnoreLayer))
                GameEvent.OnTouchBeganMethod(hit);
        }
        if (Mouse.current.leftButton.isPressed && state == RaycastState.Running)
            GameEvent.OnTouchDragMethod(Mouse.current.position.ReadValue());

        if (Mouse.current.leftButton.wasReleasedThisFrame && state == RaycastState.Running)
        {
            state = RaycastState.None;
            GameEvent.OnTouchEndedMethod(Mouse.current.position.ReadValue());
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
}
