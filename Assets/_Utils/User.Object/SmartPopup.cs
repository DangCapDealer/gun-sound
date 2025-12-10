using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class SmartPopup : MonoBehaviour
{
    public DWPopupEffect Effect = DWPopupEffect.None;
    public float Duration = 0.5f;
    public UnityEvent DWCompleteEvent;

    public void Show() => DWVisual.PlayPopupEffect(gameObject, Effect, Duration);

    // Nếu muốn tự động chạy khi enable
    private void OnEnable() => Show();
}