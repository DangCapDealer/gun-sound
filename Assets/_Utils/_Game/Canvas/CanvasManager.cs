using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    [System.Serializable]
    public struct CanvasRoot
    {
        public string CanvasID;
        public ScreenCanvas Canvas;
    }

    public CanvasRoot[] CanvasRoots;

    void OnEnable()
    {
        EventBus.Subscribe<CanvasEvent>(OnCanvasEventMethod);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<CanvasEvent>(OnCanvasEventMethod);
    }

    void OnCanvasEventMethod(CanvasEvent evt)
    {
        foreach (var canvasRoot in CanvasRoots)
        {
            if (canvasRoot.CanvasID == evt.CanvasId)
            {
                canvasRoot.Canvas.Show();
            }
            else canvasRoot.Canvas.Hide();
        }        
    }
}
