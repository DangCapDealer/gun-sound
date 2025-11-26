using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using EditorCools;
using System;

public class FloatingDamageSprites : MonoBehaviour
{
    [Header("Sprite Settings")]
    public Sprite[] digitSprites; // 0-9
    public Sprite[] specialSprites; // [0]=K, [1]=M, [2]=crit

    public float charSpacing = 0.2f;
    [Header("Animation Settings")]
    public float moveUpDistance = 1.0f;
    public float fadeOutDuration = 0.5f;
    public float totalLifetime = 1.5f;

    private List<SpriteRenderer> currentDigits = new();
    private Dictionary<char, Sprite> digitMap = new();

    void Awake()
    {
        for (int i = 0; i < 10; i++)
            if (i < digitSprites.Length && digitSprites[i])
                digitMap[(char)('0' + i)] = digitSprites[i];
    }

    [Button]
    public void Test()
    {
        DisplayDamage(UnityEngine.Random.Range(10, 10000), Vector3.zero, UnityEngine.Random.value > 0.5f);
    }

    public void DisplayDamage(int damageAmount, Vector3 startPosition, bool damageCrit)
    {
        transform.position = startPosition;
        transform.localScale = damageCrit ? Vector3.one : Vector3.one.ScaleValue(0.65f);
        transform.Show();

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

        void Spawn(Sprite sp)
        {
            if (!sp) return;
            var go = PoolManager.I.PopPool(PoolName.DigitSprite, transform);
            go.Show();
            var sr = go.GetComponent<SpriteRenderer>();
            if (sr)
            {
                sr.color = sr.color.WithAlpha(1f);
                sr.sprite = sp;
                go.transform.localPosition = VectorExtensions.Create3D(currentX);
                currentX += charSpacing;
                currentDigits.Add(sr);
            }
        }

        if (damageCrit && specialSprites?.Length > 2) Spawn(specialSprites[2]);
        foreach (var ch in digitsStr) if (digitMap.TryGetValue(ch, out var sp)) Spawn(sp);
        if (suffix != null && specialSprites != null)
        {
            int idx = suffix == 'K' ? 0 : 1;
            if (specialSprites.Length > idx) Spawn(specialSprites[idx]);
        }

        transform.DOMoveY(transform.position.y + moveUpDistance, totalLifetime).SetEase(Ease.OutQuad);
        foreach (var sr in currentDigits)
            sr.DOFade(0f, fadeOutDuration).SetDelay(totalLifetime - fadeOutDuration);

        DOVirtual.DelayedCall(totalLifetime, () =>
        {
            while (transform.childCount > 0)
                PoolManager.I.PushPool(transform.GetChild(0).gameObject, PoolName.DigitSprite);
            PoolManager.I.PushPool(gameObject, PoolName.DamageSprite);
        });
    }

    private void OnValidate()
    {
    #if UNITY_EDITOR
        // Lấy đường dẫn asset của script này
        string scriptPath = UnityEditor.AssetDatabase.GetAssetPath(UnityEditor.MonoScript.FromMonoBehaviour(this));
        string folderPath = System.IO.Path.GetDirectoryName(scriptPath);
    
        // Tìm file sprite "tmp_dmg" trong folder
        string[] files = System.IO.Directory.GetFiles(folderPath);
        string tmpDmgPath = null;
        foreach (var file in files)
        {
            if (System.IO.Path.GetFileNameWithoutExtension(file) == "tmp_dmg")
            {
                tmpDmgPath = file.Replace("\\", "/");
                break;
            }
        }
    
        if (string.IsNullOrEmpty(tmpDmgPath))
        {
            Debug.LogWarning("Không tìm thấy file sprite: tmp_dmg trong folder!");
            return;
        }
    
        // Đổi cách lấy assetPath cho đúng chuẩn Unity
        string assetPath = folderPath + "/tmp_dmg.png";
        int idx = assetPath.IndexOf("Assets", StringComparison.Ordinal);
        if (idx >= 0)
            assetPath = assetPath.Substring(idx);

        var sprites = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(assetPath);
        var digitList = new List<Sprite>();
        var specialList = new List<Sprite>();
    
        foreach (var obj in sprites)
        {
            if (obj is Sprite sp)
            {
                if (int.TryParse(sp.name, out int digit) && digit >= 0 && digit <= 9)
                    digitList.Add(sp);
                else
                    specialList.Add(sp);
            }
        }
    
        // Sắp xếp digitSprites theo thứ tự 0-9
        digitList.Sort((a, b) => string.Compare(a.name, b.name, StringComparison.Ordinal));
        digitSprites = digitList.ToArray();
        specialSprites = specialList.ToArray();
    
        Debug.Log($"Đã import {digitSprites.Length} digitSprites và {specialSprites.Length} specialSprites từ tmp_dmg.");
    #endif
    }
}