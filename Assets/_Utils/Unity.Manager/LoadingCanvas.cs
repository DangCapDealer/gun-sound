using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class LoadingCanvas : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image progress;

    public void Show(Action callback, float startValue = 0f, float endValue = 1f, float fillTime = 1f)
    {
        progress.fillAmount = startValue;
        transform.Show();
        canvasGroup.transform.Show();

        canvasGroup.DOKill();
        canvasGroup.alpha = 1f;
        canvasGroup
            .DOFade(1f, 0.2f)
            .SetUpdate(true)
            .OnComplete(() => FillImageWithDOTween(callback, startValue, endValue, fillTime));
    }

    public void Hide(float delay = 1.0f)
    {
        canvasGroup.DOKill();
        canvasGroup
            .DOFade(0f, 0.2f)
            .SetUpdate(true)
            .SetDelay(delay)
            .OnComplete(() => canvasGroup.transform.Hide());
    }

    public void FillImageWithDOTween(Action callback, float startValue = 0f, float endValue = 1f, float fillTime = 1f)
    {
        if (progress == null)
        {
            Debug.LogError("Progress Image không được gán!");
            callback?.Invoke();
            return;
        }

        progress.fillAmount = startValue;
        progress.DOKill();
        progress
            .DOFillAmount(endValue, fillTime)
            .SetEase(Ease.Linear)
            .SetUpdate(true)
            .OnComplete(() => callback?.Invoke());
    }
}
