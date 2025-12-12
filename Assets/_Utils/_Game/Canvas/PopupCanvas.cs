using DG.Tweening;
using EditorCools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CanvasGroup))]
public abstract class PopupCanvas : MonoBehaviour
{
    public virtual void Show(UnityAction _actionFirst, UnityAction _actionSecond, string[] msg)
    {
        if (gameObject.IsActive()) return;
        Initialized(_actionFirst, _actionSecond, msg);
        gameObject.Show();
    }

    protected abstract void Initialized(UnityAction _actionFirst, UnityAction _actionSecond, string[] msg);

    public virtual void Hide()
    {
        if (!gameObject.IsActive()) return;
        gameObject.SetActive(false);
    }

    public bool IsActive() => gameObject.IsActive();
}
