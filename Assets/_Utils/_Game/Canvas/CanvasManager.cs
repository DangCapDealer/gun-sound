using UnityEngine;

public class CanvasManager : Singleton<CanvasManager>
{
    [System.Serializable]
    public struct CanvasRoot
    {
        public string CanvasID;
        public ScreenCanvas Canvas;
    }

    [System.Serializable]
    public struct PopupRoot
    {
        public string PopupID;
        public PopupCanvas Popup;
    }

    [Header("Canvas & Popup Roots")]
    [SerializeField] private CanvasRoot[] CanvasRoots;
    [SerializeField] private PopupRoot[] PopupRoots;


    [Header("Identifiers")]
    [SerializeField] private string _canvasId = "";
    [SerializeField] private string _popupId = "";

    public string CanvasId => _canvasId;
    public string PopupId => _popupId;

    protected virtual void OnEnable()
    {
        EventBus.Subscribe<CanvasEvent>(OnCanvasEventMethod);
        EventBus.Subscribe<PopupEvent>(OnPopupEventMethod);
    }

    protected virtual void OnDisable()
    {
        EventBus.Unsubscribe<CanvasEvent>(OnCanvasEventMethod);
        EventBus.Unsubscribe<PopupEvent>(OnPopupEventMethod);
    }

    public virtual void OnCanvasEventMethod(CanvasEvent evt)
    {
        _canvasId = evt.CanvasId;
        foreach (var canvasRoot in CanvasRoots)
        {
            if (canvasRoot.CanvasID == evt.CanvasId)
            {
                canvasRoot.Canvas.Show();
            }
            else canvasRoot.Canvas.Hide();
        }        
    }

    public virtual void OnPopupEventMethod(PopupEvent evt)
    {
        _popupId = evt.PopupId;
        foreach (var popupRoot in PopupRoots)
        {
            if (popupRoot.PopupID == evt.PopupId)
            {
                popupRoot.Popup.Show(null, null, null);
            }
            else popupRoot.Popup.Hide();
        }
    }
}
