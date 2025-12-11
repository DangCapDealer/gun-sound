#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Linq;

public static class SpriteSheetLoader
{
    public static Sprite[] LoadLargeSprites(string relativePath, int minWidth = 32, int minHeight = 32)
    {
        Debug.Log($"[SpriteSheetLoader] Đang load asset tại: {relativePath}");
        var assets = AssetDatabase.LoadAllAssetsAtPath(relativePath);
        Debug.Log($"[SpriteSheetLoader] Tổng asset load được: {assets.Length}");

        var sprites = assets
            .OfType<Sprite>()
            .Where(s =>
            {
                bool ok = s.rect.width >= minWidth && s.rect.height >= minHeight;
                if (!ok)
                    Debug.Log($"[SpriteSheetLoader] Sprite nhỏ bị bỏ qua: {s.name} ({s.rect.width}x{s.rect.height})");
                else
                    Debug.Log($"[SpriteSheetLoader] Sprite hợp lệ: {s.name} ({s.rect.width}x{s.rect.height})");
                return ok;
            })
            .ToArray();

        Debug.Log($"[SpriteSheetLoader] Số sprite hợp lệ: {sprites.Length}");
        return sprites;
    }
}
#endif