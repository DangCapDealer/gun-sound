using UnityEngine;
using UnityEditor;
using System.IO;
using System.Threading.Tasks;

public class PNGResizer : EditorWindow
{
    private Texture2D sourceTexture;
    private int newWidth = 256;
    private int newHeight = 256;

    [MenuItem("Tools/Resize PNG")]
    public static void ShowWindow()
    {
        GetWindow<PNGResizer>("Resize PNG");
    }

    //[MenuItem("Tools/Resize PNG")]
    //public static void GetSelectedFiles()
    //{
    //    foreach (var obj in Selection.objects)
    //    {
    //        string path = AssetDatabase.GetAssetPath(obj);
    //        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);

    //        if (texture == null) break;
    //        int originalWidth = texture.width;
    //        int originalHeight = texture.height;

    //        int adjustedWidth = Mathf.RoundToInt(originalWidth / 4f) * 4;
    //        int adjustedHeight = Mathf.RoundToInt(originalHeight / 4f) * 4;

    //        Debug.Log($"Texture: {texture.name} | Original: {originalWidth}x{originalHeight} | Adjusted: {adjustedWidth}x{adjustedHeight} at {path}");
    //    }
    //}

    private void OnGUI()
    {
        if(GUILayout.Button("Checking Files"))
        {
            foreach (var obj in Selection.objects)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);

                if (texture == null) break;
                int originalWidth = texture.width;
                int originalHeight = texture.height;

                int adjustedWidth = Mathf.RoundToInt(originalWidth / 4f) * 4;
                int adjustedHeight = Mathf.RoundToInt(originalHeight / 4f) * 4;

                Debug.Log($"Texture: {texture.name} | Original: {originalWidth}x{originalHeight} | Adjusted: {adjustedWidth}x{adjustedHeight} at {path}");
            }
        }

        GUILayout.Label("Resize PNG File", EditorStyles.boldLabel);
        sourceTexture = (Texture2D)EditorGUILayout.ObjectField("Source PNG", sourceTexture, typeof(Texture2D), false);
        newWidth = EditorGUILayout.IntField("New Width", newWidth);
        newHeight = EditorGUILayout.IntField("New Height", newHeight);

        if (GUILayout.Button("Resize & Save"))
        {
            if (sourceTexture != null)
            {
                ResizeAndSavePNG(sourceTexture, newWidth, newHeight);
            }
            else
            {
                Debug.LogError("No PNG selected!");
            }
        }
    }

    static async void ResizeImageAsync()
    {
        Debug.Log("Task Start");
        await Task.Delay(2000); // Giống như WaitForSeconds(2)
        Debug.Log("Task End");
    }

    private void ResizeAndSavePNG(Texture2D source, int width, int height)
    {
        string assetPath = AssetDatabase.GetAssetPath(source);
        if (string.IsNullOrEmpty(assetPath))
        {
            Debug.LogError("Could not find the asset path!");
            return;
        }

        RenderTexture rt = RenderTexture.GetTemporary(width, height);
        Graphics.Blit(source, rt);

        RenderTexture.active = rt;
        Texture2D resized = new Texture2D(width, height, TextureFormat.RGBA32, false);
        resized.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        resized.Apply();

        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);

        byte[] pngData = resized.EncodeToPNG();
        File.WriteAllBytes(assetPath, pngData);
        AssetDatabase.Refresh();

        Debug.Log($"Resized PNG saved at: {assetPath}");
    }
}
