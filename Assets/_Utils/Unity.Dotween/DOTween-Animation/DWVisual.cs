using DG.Tweening;
using UnityEngine;

public enum DWPopupEffect
{
    None,
    Punch,
    MoveFromLeft,
    MoveFromRight,
    MoveFromTop,
    MoveFromBottom,
    ScaleIn,
    FadeIn
}

public static class DWVisual
{
    public static void PlayPopupEffect(GameObject popup, DWPopupEffect effect, float duration = 0.5f, System.Action onComplete = null)
    {
        var rect = popup.transform as RectTransform;
        Tween tween = null;

        switch (effect)
        {
            case DWPopupEffect.None:
                popup.SetActive(true);
                onComplete?.Invoke();
                return;

            case DWPopupEffect.Punch:
                popup.SetActive(true);
                rect.localScale = Vector3.one * 0.8f;
                tween = rect.DOScale(Vector3.one, duration).SetEase(Ease.OutBack);
                break;

            case DWPopupEffect.MoveFromLeft:
                popup.SetActive(true);
                rect.anchoredPosition = new Vector2(-Screen.width, rect.anchoredPosition.y);
                tween = rect.DOAnchorPosX(0, duration).SetEase(Ease.OutCubic);
                break;

            case DWPopupEffect.MoveFromRight:
                popup.SetActive(true);
                rect.anchoredPosition = new Vector2(Screen.width, rect.anchoredPosition.y);
                tween = rect.DOAnchorPosX(0, duration).SetEase(Ease.OutCubic);
                break;

            case DWPopupEffect.MoveFromTop:
                popup.SetActive(true);
                rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, Screen.height);
                tween = rect.DOAnchorPosY(0, duration).SetEase(Ease.OutCubic);
                break;

            case DWPopupEffect.MoveFromBottom:
                popup.SetActive(true);
                rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, -Screen.height);
                tween = rect.DOAnchorPosY(0, duration).SetEase(Ease.OutCubic);
                break;

            case DWPopupEffect.ScaleIn:
                popup.SetActive(true);
                rect.localScale = Vector3.zero;
                tween = rect.DOScale(Vector3.one, duration).SetEase(Ease.OutBack);
                break;

            case DWPopupEffect.FadeIn:
                popup.SetActive(true);
                var cg = popup.GetComponent<CanvasGroup>();
                if (cg == null) cg = popup.AddComponent<CanvasGroup>();
                cg.alpha = 0;
                tween = cg.DOFade(1, duration);
                break;
        }

        if (tween != null)
            tween.OnComplete(() => onComplete?.Invoke());
    }
}