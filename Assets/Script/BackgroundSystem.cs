using UnityEngine;
using DG.Tweening;

public class BackgroundSystem : MonoBehaviour
{
    public class BackgroundChangedEvent { public string BackgroundId; }

    [System.Serializable]
    public class BackgroundSettings
    {
        public string BackgroundId;
        public Sprite BackgroundSprite;
    }

    [Header("Background Settings")]
    public BackgroundSettings[] Backgrounds;

    [Header("Sprite Renderers")]
    public SpriteRenderer backgroundA;
    public SpriteRenderer backgroundB;

    private bool useA = true;
    private bool isFading = false;
    private float fadeDuration = 0.5f;

    void OnEnable()
    {
        EventBus.Subscribe<BackgroundChangedEvent>(OnBackgroundChanged);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<BackgroundChangedEvent>(OnBackgroundChanged);
    }

    private void OnBackgroundChanged(BackgroundChangedEvent evt)
    {
        if (isFading) return; // Chặn nếu đang fade
        isFading = true;

        var bg = System.Array.Find(Backgrounds, b => b.BackgroundId == evt.BackgroundId);
        if (bg == null)
        {
            isFading = false;
            return;
        }

        // Chọn renderer để fade
        SpriteRenderer fadeOut = useA ? backgroundA : backgroundB;
        SpriteRenderer fadeIn = useA ? backgroundB : backgroundA;

        fadeIn.sprite = bg.BackgroundSprite;
        fadeIn.color = fadeIn.color.WithAlpha(0);
        fadeIn.gameObject.SetActive(true);

        fadeIn.DOFade(1, fadeDuration);
        fadeOut.DOFade(0, fadeDuration).OnComplete(() =>
        {
            fadeOut.gameObject.SetActive(false);
            isFading = false; // Cho phép đổi nền tiếp
        });

        useA = !useA;
    }
}

public static class BackgroundSystemExtensions
{
    private static BackgroundSystem.BackgroundChangedEvent _backgroundChangedEvent = new BackgroundSystem.BackgroundChangedEvent();

    public static BackgroundSystem.BackgroundChangedEvent ChooseBackgroudMethod(this string backgroundId)
    {
        _backgroundChangedEvent.BackgroundId = backgroundId;
        return _backgroundChangedEvent;
    }
}
