using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.Build.Reporting;
using System.Linq;
using UnityEditor.SceneManagement;
using Unity.VisualScripting;
using System;

public class CustomBuildMenu
{
    private const string CORRECT_PASSWORD = "123456";

    [MenuItem("Build/Setup Build Password")]
    public static void SetupPassword()
    {
        PlayerSettings.Android.keystorePass = CORRECT_PASSWORD;
        PlayerSettings.Android.keyaliasPass = CORRECT_PASSWORD;
    }
}