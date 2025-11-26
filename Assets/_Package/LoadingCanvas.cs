using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class LoadingCanvas : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Transform characterDemo;
    [SerializeField] private Image progress;

    public void Show(Action CALLBACK, float startValue = 0.0f, float endValue = 1.0f, float fillTime = 1.0f)
    {
        progress.fillAmount = 0;

        this.transform.Show();
        this.characterDemo.Show();
        this.canvasGroup.transform.Show();

        this.canvasGroup.alpha = 1f;
        this.canvasGroup.DOKill();
        this.canvasGroup
            .DOFade(1f, 0.2f)
            .SetUpdate(true) // chạy unscaled time
            .OnComplete(() =>
            {
                Debug.Log("[LoadingCanvas] Fade complete");
                FillImageWithDOTween(CALLBACK, startValue, endValue, fillTime);
            });

        Debug.Log("[LoadingCanvas] Show loading ad");
    }

    public void Hide()
    {
        this.characterDemo.Hide();
        this.canvasGroup.DOKill();
        this.canvasGroup
            .DOFade(0.0f, 0.2f)
            .SetUpdate(true) // chạy unscaled time
            .OnComplete(() => this.canvasGroup.transform.Hide());

        Debug.Log("[LoadingCanvas] Hide loading ad");
    }

    //public void Show(Action CALLBACK, float startValue = 0.0f, float endValue = 1.0f, float fillSpeed = 1.0f, float fillTime = 1.0f)
    //{
    //    FillImageWithDOTween(CALLBACK, startValue, endValue, fillTime);
    //}

    public void FillImageWithDOTween(Action CALLBACK, float startValue = 0.0f, float endValue = 1.0f, float fillTime = 1.0f)
    {
        if (progress == null)
        {
            Debug.LogError("Progress Image không được gán! Không thể tween.");
            CALLBACK?.Invoke();
            return;
        }

        progress.fillAmount = startValue;
        progress.DOKill();
        progress
            .DOFillAmount(endValue, fillTime)
            .SetEase(Ease.Linear)
            .SetUpdate(true) // chạy unscaled time
            .OnUpdate(() => {
                // Debug.Log($"[LoadingCanvas] Progress fill amount {progress.fillAmount}");
            })
            .OnComplete(() => {
                CALLBACK?.Invoke();
                // Debug.Log("FillImageWithDOTween hoàn tất!");
            });
    }
}
