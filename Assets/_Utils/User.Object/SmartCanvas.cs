using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CanvasGroup))]
public class SmartCanvas : MonoBehaviour
{
    public bool AutoFadeInOnEnable = false;
    public float FadeDuration = 0.3f;
    public UnityEvent OnFadeComplete;

    private CanvasGroup _canvasGroup;

    private void OnEnable()
    {
        if (AutoFadeInOnEnable) FadeIn();
    }

    private void OnDisable() => FadeOut();

    public void FadeIn()
    {
        _canvasGroup ??= GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0f;
        _canvasGroup.DOFade(1f, FadeDuration).OnComplete(() => OnFadeComplete?.Invoke());
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
    }

    public void FadeOut()
    {
        _canvasGroup ??= GetComponent<CanvasGroup>();
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }
}