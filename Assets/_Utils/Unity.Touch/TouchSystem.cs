using UnityEngine;
using UnityEngine.EventSystems;

public class TouchSystem : MonoBehaviour
{
    public bool IsDebug = false;
    public bool IsTouching { get; private set; }
    public Vector2 TouchPosition { get; private set; }
    public bool IsHolding { get; private set; }

    void Update()
    {
        // Nếu bấm vào UI thì block lại
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            if (IsTouching)
                if (IsDebug) Debug.Log("[TouchSystem] Blocked by UI");
            IsTouching = false;
            IsHolding = false;
            return;
        }

#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            IsTouching = true;
            IsHolding = false;
            TouchPosition = Input.mousePosition;
            if (IsDebug) Debug.Log($"[TouchSystem] Mouse Down at {TouchPosition}");
        }
        else if (Input.GetMouseButton(0))
        {
            if (IsTouching && !IsHolding)
            {
                IsHolding = true;
                if (IsDebug) Debug.Log($"[TouchSystem] Mouse Hold at {Input.mousePosition}");
            }
            TouchPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            IsTouching = false;
            if (IsHolding)
                if (IsDebug) Debug.Log("[TouchSystem] Mouse Hold End");
            IsHolding = false;
            if (IsDebug) Debug.Log("[TouchSystem] Mouse Up");
        }
#elif UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount == 1)
        {
            var touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                IsTouching = true;
                IsHolding = false;
                TouchPosition = touch.position;
                if (IsDebug) Debug.Log($"[TouchSystem] Touch Began at {TouchPosition}");
            }
            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                if (IsTouching && !IsHolding)
                {
                    IsHolding = true;
                    if (IsDebug) Debug.Log($"[TouchSystem] Touch Hold at {touch.position}");
                }
                TouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                IsTouching = false;
                if (IsHolding)
                    if (IsDebug) Debug.Log("[TouchSystem] Touch Hold End");
                IsHolding = false;
                if (IsDebug) Debug.Log("[TouchSystem] Touch Ended");
            }
        }
        else
        {
            if (IsTouching || IsHolding)
                if (IsDebug) Debug.Log("[TouchSystem] Touch count != 1, reset");
            IsTouching = false;
            IsHolding = false;
        }
#endif
    }
}
