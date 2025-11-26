using UnityEditor;
using UnityEngine;

public class ObjectMenu : EditorWindow
{
    private GameObject selectedObject;

    [MenuItem("Tools/Object Menu")]
    public static void ShowWindow()
    {
        GetWindow<ObjectMenu>("Object Menu");
    }

    private void OnGUI()
    {
        GUILayout.Label("Virtual Pivot Generator", EditorStyles.boldLabel);

        selectedObject = Selection.activeGameObject;

        if (selectedObject != null)
        {
            EditorGUILayout.LabelField("Object đang chọn:", selectedObject.name);
        }
        else
        {
            EditorGUILayout.LabelField("Không có Object nào được chọn.");
        }
    }
}
