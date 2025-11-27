using UnityEngine;

public interface IRaycastEventsListener
{
    void OnRaycastBegan(RaycastHit hit);
    void OnRaycastBeganPosition(Vector2 position);
    void OnRaycastDrag(Vector2 position);
    void OnRaycastEnded(Vector2 position);
    void OnRaycastBeganPauseState(Transform hitTransform);
    void OnRaycastMovePauseState(Vector2 position);
    void OnRaycastEndPauseState(Vector2 position);
}