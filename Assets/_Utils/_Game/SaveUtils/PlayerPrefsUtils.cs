using UnityEngine;

/// <summary>
/// Tiện ích mở rộng cho PlayerPrefs: hỗ trợ bool, int, float, string.
/// </summary>
public static class PlayerPrefsUtils
{
    // Bool
    public static void SetBool(string key, bool value) => PlayerPrefs.SetInt(key, value ? 1 : 0);
    public static bool GetBool(string key, bool defaultValue = false) => PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) != 0;

    // Int
    public static void SetInt(string key, int value) => PlayerPrefs.SetInt(key, value);
    public static int GetInt(string key, int defaultValue = 0) => PlayerPrefs.GetInt(key, defaultValue);

    // Float
    public static void SetFloat(string key, float value) => PlayerPrefs.SetFloat(key, value);
    public static float GetFloat(string key, float defaultValue = 0f) => PlayerPrefs.GetFloat(key, defaultValue);

    // String
    public static void SetString(string key, string value) => PlayerPrefs.SetString(key, value);
    public static string GetString(string key, string defaultValue = "") => PlayerPrefs.GetString(key, defaultValue);

    // Xóa key
    public static void DeleteKey(string key) => PlayerPrefs.DeleteKey(key);

    // Kiểm tra tồn tại
    public static bool HasKey(string key) => PlayerPrefs.HasKey(key);
}