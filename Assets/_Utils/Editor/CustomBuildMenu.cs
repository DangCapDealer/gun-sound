using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.Build.Reporting;
using System.Linq;
using UnityEditor.SceneManagement;
using Unity.VisualScripting;
using System;

[CustomEditor(typeof(BuildPasswordConfig))]
public class BuildPasswordConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Default Company
        EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultCompany"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("keystorePassword"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("keyaliasPassword"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("packageName"));

        // Version row with "Next Version" button
        EditorGUILayout.BeginHorizontal();
        var versionProp = serializedObject.FindProperty("version");
        EditorGUILayout.PropertyField(versionProp);
        if (GUILayout.Button("→", GUILayout.Width(30)))
        {
            versionProp.stringValue = NextVersion(versionProp.stringValue);
            var config = (BuildPasswordConfig)target;
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        EditorGUILayout.EndHorizontal();

        // BundleVersionCode row with "Next Version Code" button
        EditorGUILayout.BeginHorizontal();
        var codeProp = serializedObject.FindProperty("bundleVersionCode");
        EditorGUILayout.PropertyField(codeProp);
        if (GUILayout.Button("→", GUILayout.Width(30)))
        {
            codeProp.intValue += 1;
            var config = (BuildPasswordConfig)target;
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        EditorGUILayout.EndHorizontal();

        // Nút "Set Config" để setup PlayerSettings từ ScriptableObject
        EditorGUILayout.Space();
        if (GUILayout.Button("Set Build Password & Android Info"))
        {
            var config = (BuildPasswordConfig)target;
            PlayerSettings.Android.keystorePass = config.keystorePassword;
            PlayerSettings.Android.keyaliasPass = config.keyaliasPassword;
            PlayerSettings.applicationIdentifier = config.packageName;
            PlayerSettings.bundleVersion = config.version;
            PlayerSettings.Android.bundleVersionCode = config.bundleVersionCode;
            PlayerSettings.companyName = config.defaultCompany;

            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            // EditorUtility.DisplayDialog("Build Password", "Đã setup password, Android info và company name từ ScriptableObject!", "OK");
        }

        serializedObject.ApplyModifiedProperties();
    }

    // Tăng version: 1.0.0 -> 1.0.1
    private string NextVersion(string version)
    {
        var parts = version.Split('.');
        if (parts.Length == 3 &&
            int.TryParse(parts[0], out int major) &&
            int.TryParse(parts[1], out int minor) &&
            int.TryParse(parts[2], out int patch))
        {
            patch++;
            if (patch > 9)
            {
                patch = 0;
                minor++;
                if (minor > 9)
                {
                    minor = 0;
                    major++;
                }
            }
            return $"{major}.{minor}.{patch}";
        }
        return version;
    }
}

[CreateAssetMenu(fileName = "BuildPasswordConfig", menuName = "Build/Password Config")]
public class BuildPasswordConfig : ScriptableObject
{
    [Header("Android Settings")]
    public string defaultCompany = "DefaultCompany";
    public string keystorePassword = "123456";
    public string keyaliasPassword = "123456";
    public string packageName = "com.company.product";
    public string version = "1.0.0";
    public int bundleVersionCode = 1;
}

public class CustomBuildMenu
{
    private const string CONFIG_PATH = "Assets/_Utils/Editor/BuildPasswordConfig.asset";

    [MenuItem("Build/Select Build Config %&b")]
    public static void SelectBuildConfig()
    {
        var config = AssetDatabase.LoadAssetAtPath<BuildPasswordConfig>(CONFIG_PATH);
        if (config != null)
        {
            Selection.activeObject = config;
        }
        else
        {
            EditorUtility.DisplayDialog("Build Password", "Không tìm thấy BuildPasswordConfig.asset! Vui lòng tạo asset này trước.", "OK");
        }
    }

    [MenuItem("Build/Setup Build Password & Android Info")]
    public static void SetupPasswordAndAndroidInfo()
    {
        var config = AssetDatabase.LoadAssetAtPath<BuildPasswordConfig>(CONFIG_PATH);
        if (config == null)
        {
            EditorUtility.DisplayDialog("Build Password", "Không tìm thấy BuildPasswordConfig.asset! Vui lòng tạo asset này trước.", "OK");
            return;
        }
        PlayerSettings.Android.keystorePass = config.keystorePassword;
        PlayerSettings.Android.keyaliasPass = config.keyaliasPassword;
        PlayerSettings.applicationIdentifier = config.packageName;
        PlayerSettings.bundleVersion = config.version;
        PlayerSettings.Android.bundleVersionCode = config.bundleVersionCode;
        PlayerSettings.companyName = config.defaultCompany;
        // EditorUtility.DisplayDialog("Build Password", "Đã setup password, Android info và company name từ ScriptableObject!", "OK");
    }
}