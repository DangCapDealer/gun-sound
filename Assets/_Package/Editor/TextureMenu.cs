using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TextureMenu : EditorWindow
{
    private Texture2D selectedTexture;

    //[MenuItem("Tools/Texture Menu")]
    public static void ShowWindow()
    {
        GetWindow<ObjectMenu>("Texture Menu");
    }

    private void OnGUI()
    {
        GUILayout.Label("Virtual Texture Generator", EditorStyles.boldLabel);

        // Lấy object đang được chọn trong Project window
        Object activeObject = Selection.activeObject;

        // Kiểm tra xem object đó có phải là Texture2D không
        if (activeObject is Texture2D texture)
        {
            selectedTexture = texture;

            EditorGUILayout.LabelField("Texture đang chọn:", selectedTexture.name);

            if (GUILayout.Button("Chỉnh sửa texture"))
            {
                Debug.Log($"Đang xử lý texture: {selectedTexture.name}");
                // Thêm code xử lý tại đây
            }
        }
        else
        {
            EditorGUILayout.LabelField("Không có Texture nào được chọn.");
        }
    }
}
