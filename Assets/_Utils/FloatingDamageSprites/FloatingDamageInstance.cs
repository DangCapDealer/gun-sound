using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using System;

public class FloatingDamageInstance : MonoBehaviour
{
    [Header("Sprite Settings")]
    public Sprite[] digitSprites; // 0-9
    // specialSprites: [0]=+, [1]=-, [2]=., [3]=B, [4]=icon_crit, [5]=K, [6]=M
    public Sprite[] specialSprites; // Đặt đúng thứ tự này!
    public GameObject digitSpritePrefab; // Prefab có SpriteRenderer

    public float charSpacing = 0.2f;
    [Header("Animation Settings")]
    public float moveUpDistance = 1.0f;
    public float fadeOutDuration = 0.5f;
    public float totalLifetime = 1.5f;

    private List<SpriteRenderer> currentDigits = new();
    private Dictionary<char, Sprite> digitMap = new();
    private readonly Queue<GameObject> digitPool = new();

    void Awake()
    {
        digitMap.Clear();
        for (int i = 0; i < 10; i++)
            if (i < digitSprites.Length && digitSprites[i])
                digitMap[(char)('0' + i)] = digitSprites[i];
    }

    public void Show(int damageAmount, Vector3 startPosition, bool damageCrit, Action onDespawn)
    {
        transform.position = startPosition;
        transform.localScale = damageCrit ? Vector3.one : Vector3.one * 0.65f;
        gameObject.SetActive(true);

        // Compact number
        string digitsStr = damageAmount >= 1_000_000 ? (damageAmount / 1_000_000).ToString()
                          : damageAmount >= 1_000 ? (damageAmount / 1_000).ToString()
                          : damageAmount.ToString();
        char? suffix = damageAmount >= 1_000_000 ? 'M'
                     : damageAmount >= 1_000 ? 'K'
                     : (char?)null;

        int glyphCount = digitsStr.Length + (suffix != null ? 1 : 0) + (damageCrit ? 1 : 0);
        float currentX = -glyphCount * charSpacing / 2f;
        currentDigits.Clear();

        void Spawn(Sprite sp, bool isSpecial = false)
        {
            if (!sp) return;
            GameObject go = null;
            while (digitPool.Count > 0)
            {
                var candidate = digitPool.Dequeue();
                if (!candidate.activeSelf)
                {
                    go = candidate;
                    break;
                }
            }
            if (go == null)
                go = Instantiate(digitSpritePrefab, transform);

            go.SetActive(true);
            var sr = go.GetComponent<SpriteRenderer>();
            if (sr)
            {
                sr.color = sr.color.WithAlpha(1f);
                sr.sprite = sp;
                go.transform.localPosition = VectorExtensions.Create3D(currentX);
                if (isSpecial) currentX += charSpacing * 0.3f; 
                else currentX += charSpacing;
                currentDigits.Add(sr);
            }
        }

        if (damageCrit && specialSprites?.Length > 2) Spawn(specialSprites[4]);
        foreach (var ch in digitsStr) if (digitMap.TryGetValue(ch, out var sp)) Spawn(sp);
        if (suffix != null && specialSprites != null)
        {
            int idx = -1;
            switch(suffix)
            {
                case '+': idx = 0; break;
                case '-': idx = 1; break;
                case '.': idx = 2; break;
                case 'B': idx = 3; break;
                case 'K': idx = 5; break;
                case 'M': idx = 6; break;
            }
            if (idx != -1 && specialSprites.Length > idx) Spawn(specialSprites[idx]);
        }

        transform.DOMoveY(transform.position.y + moveUpDistance, totalLifetime).SetEase(Ease.OutQuad);
        foreach (var sr in currentDigits)
            sr.DOFade(0f, fadeOutDuration).SetDelay(totalLifetime - fadeOutDuration);

        DOVirtual.DelayedCall(totalLifetime, () =>
        {
            foreach (var sr in currentDigits)
            {
                sr.DOFade(1f, 0);
                sr.gameObject.SetActive(false);
                digitPool.Enqueue(sr.gameObject);
            }
            currentDigits.Clear();
            onDespawn?.Invoke();
        });
    }
}