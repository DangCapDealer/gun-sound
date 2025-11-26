using DG.Tweening;
using EditorCools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CanvasGroup))]
public abstract class PopupCanvas : MonoBehaviour
{
    public Popup popupID;
    public virtual void Show(Popup p, UnityAction _actionFirst, UnityAction _actionSecond, string[] msg)
    {
        if (popupID == p)
        {
            if (gameObject.IsActive()) return;
            gameObject.Show();
            Initialized(_actionFirst, _actionSecond, msg);
        }
        else Hide();
    }

    protected abstract void Initialized(UnityAction _actionFirst, UnityAction _actionSecond, string[] msg);

    public virtual void Hide()
    {
        if (!gameObject.IsActive()) return;
        HidePopup();
    }

    public bool IsActive() => gameObject.IsActive();

    public enum PopupAnimationType
    {
        None,             
        PunchScaleContent, 
        FadeInScaleOutParent,
        SlideInFromTopParent, 
        GrowFromCenterParent  
    }

    [Header("Animation Settings")]
    public PopupAnimationType animationType = PopupAnimationType.PunchScaleContent; 

    [Tooltip("Cần có CanvasGroup trên GameObject này (thằng cha) để điều khiển alpha.")]
    public CanvasGroup canvasGroup; 
    [Tooltip("Panel chứa nội dung chính của popup (thằng con), nơi sẽ áp dụng PunchScale.")]
    public RectTransform popupContent;

    public float animationDuration = 0.3f;
    public Ease easeType = Ease.OutBack; 

    public float punchScaleAmount = 0.1f;
    public float punchDuration = 0.5f;  
    public int punchVibrato = 10;     
    public float punchElasticity = 1f; 

    public float fadeInScaleAmountParent = 0.8f; 
    public float fadeOutScaleAmountParent = 1.05f;

    public float slideOffsetParent = 500f; 

    private Vector3 parentOriginalScale;
    private Vector3 parentOriginalPosition;
    private Vector3 popupContentOriginalScale; 
    private Coroutine currentAnimationCoroutine;

    void Awake()
    {
        parentOriginalScale = transform.localScale;
        parentOriginalPosition = transform.localPosition; 

        if (popupContent != null)
        {
            popupContentOriginalScale = popupContent.localScale;
        }
        else
        {
            Debug.LogError("PopupAnimator: 'Popup Content' (RectTransform) chưa được gán. Vui lòng kéo panel nội dung vào.");
        }
    }

    void OnEnable()
    {
        if (currentAnimationCoroutine != null) StopCoroutine(currentAnimationCoroutine);
        transform.DOKill(true); 
        if (popupContent != null) popupContent.DOKill(true); 

        ResetInitialState();
        PlaySelectedAnimation();
    }

    void ResetInitialState()
    {
        transform.localScale = parentOriginalScale;
        transform.localPosition = parentOriginalPosition;
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        }

        if (popupContent != null)
        {
            popupContent.localScale = popupContentOriginalScale;
            // popupContent.localPosition = popupContentOriginalPosition; // Tùy nếu bạn có dịch chuyển nội dung con
        }

        switch (animationType)
        {
            case PopupAnimationType.FadeInScaleOutParent:
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 0f;
                    canvasGroup.blocksRaycasts = false; 
                    canvasGroup.interactable = false;
                }
                transform.localScale = Vector3.one * fadeInScaleAmountParent; 
                break;
            case PopupAnimationType.SlideInFromTopParent:
                transform.localPosition = parentOriginalPosition + new Vector3(0, slideOffsetParent, 0);
                break;
            case PopupAnimationType.GrowFromCenterParent:
                transform.localScale = Vector3.zero;
                break;
            case PopupAnimationType.PunchScaleContent:
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 1f;
                    canvasGroup.blocksRaycasts = true;
                    canvasGroup.interactable = true;
                }
                transform.localScale = parentOriginalScale;
                transform.localPosition = parentOriginalPosition;
                break;
        }
    }


    public void PlaySelectedAnimation()
    {
        transform.DOKill(true);
        if (popupContent != null) popupContent.DOKill(true);

        switch (animationType)
        {
            case PopupAnimationType.PunchScaleContent:
                if (popupContent == null) return;
                popupContent.DOPunchScale(
                    new Vector3(punchScaleAmount, punchScaleAmount, punchScaleAmount),
                    punchDuration,  
                    punchVibrato,  
                    punchElasticity 
                ).SetEase(Ease.OutQuad).OnComplete(() => CompleteAnimationPopup());
                break;

            case PopupAnimationType.FadeInScaleOutParent:
                if (canvasGroup == null) return;
                canvasGroup.DOFade(1f, animationDuration).SetEase(easeType);
                transform.DOScale(parentOriginalScale, animationDuration)
                    .SetEase(easeType)
                    .OnComplete(() => {
                        transform.DOScale(fadeOutScaleAmountParent * parentOriginalScale, animationDuration * 0.5f)
                                 .SetEase(Ease.OutQuad)
                                 .OnComplete(() => {
                                     transform.DOScale(parentOriginalScale, animationDuration * 0.5f)
                                              .SetEase(Ease.OutBack).OnComplete(() => CompleteAnimationPopup());
                                 });
                        canvasGroup.blocksRaycasts = true;
                        canvasGroup.interactable = true;
                    });
                break;

            case PopupAnimationType.SlideInFromTopParent:
                transform.DOLocalMove(parentOriginalPosition, animationDuration)
                    .SetEase(easeType)
                    .OnComplete(() => {
                        if (popupContent != null)
                        {
                            popupContent.DOPunchScale(
                                new Vector3(0.05f, 0.05f, 0.05f), 
                                0.2f,
                                5,
                                1f
                            ).SetEase(Ease.OutQuad).OnComplete(() => CompleteAnimationPopup());
                        }
                    });
                break;

            case PopupAnimationType.GrowFromCenterParent:
                transform.DOScale(parentOriginalScale, animationDuration)
                    .SetEase(easeType)
                    .OnComplete(() => {
                        if (popupContent != null)
                        {
                            popupContent.DOPunchScale(
                                new Vector3(0.05f, 0.05f, 0.05f),
                                0.2f,
                                5,
                                1f
                            ).SetEase(Ease.OutQuad).OnComplete(() => CompleteAnimationPopup());
                        }
                    });
                break;

            case PopupAnimationType.None:
                transform.localScale = parentOriginalScale;
                transform.localPosition = parentOriginalPosition;
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 1f;
                    canvasGroup.blocksRaycasts = true;
                    canvasGroup.interactable = true;
                }
                if (popupContent != null) popupContent.localScale = popupContentOriginalScale;
                CompleteAnimationPopup();
                break;
        }
    }

    public void HidePopup()
    {
        transform.DOKill(true);
        if (popupContent != null) popupContent.DOKill(true);
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }

        switch (animationType)
        {
            case PopupAnimationType.FadeInScaleOutParent:
                if (canvasGroup != null)
                {
                    canvasGroup.DOFade(0f, animationDuration).SetEase(Ease.InQuad)
                               .OnComplete(() => OnCompleteHidePopup());
                }
                else
                {
                    OnCompleteHidePopup();
                }
                break;
            case PopupAnimationType.SlideInFromTopParent:
                transform.DOLocalMove(parentOriginalPosition + new Vector3(0, slideOffsetParent, 0), animationDuration)
                    .SetEase(Ease.InQuad)
                    .OnComplete(() => OnCompleteHidePopup());
                break;
            case PopupAnimationType.GrowFromCenterParent:
                transform.DOScale(Vector3.zero, animationDuration).SetEase(Ease.InQuad)
                         .OnComplete(() => OnCompleteHidePopup());
                break;
            default:
                OnCompleteHidePopup();
                break;
        }

        if (popupContent != null) popupContent.localScale = popupContentOriginalScale;
    }

    protected virtual void CompleteAnimationPopup() { }
    protected virtual void OnCompleteHidePopup()
    {
        gameObject.SetActive(false);
    }
    //protected abstract void OnCompleteAnimationPopup();
}
