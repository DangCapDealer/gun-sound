using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static UnityEditorInternal.VersionControl.ListControl;
using Pathfinding;

public class AStarPathFinderObjectMenu : EditorWindow
{

    private Vector2 scrollPos;

    [MenuItem("Tools/AStar Object Menu")]
    public static void ShowWindow()
    {
        GetWindow<AStarPathFinderObjectMenu>("AStar Object Menu");
    }

    private void OnGUI()
    {
        GUILayout.Label("AStarObject Connector Menu", EditorStyles.boldLabel);
        GameObject[] selectedObjects = Selection.gameObjects;

        if(selectedObjects.Length >= 2)
        {
            EditorGUILayout.LabelField($"Có {selectedObjects.Length} được chọn.");
            if (GUILayout.Button("Kết nối"))
            {
                ConnectObjects(selectedObjects);
            }
        }   
        else
        {
            EditorGUILayout.LabelField("Không có Object nào được chọn.");
        }

        if (Event.current.type == EventType.Repaint)
        {
            Repaint(); // refresh mỗi frame (có thể bỏ nếu muốn tối ưu hơn)
        }
    }

    private void ConnectObjects(GameObject[] selectedObjects)
    {
        selectedObjects.ForEach(x =>
        {
            var scriptStart  = x.GetComponent<PointNode>();
            if (scriptStart != null)
            {
                selectedObjects.ForEach(y =>
                {
                    var scriptEnd = y.GetComponent<PointNode>();
                    if (scriptEnd != null)
                    {
                        if (x != y && scriptStart.neighbors.Contains(scriptEnd) == false)
                        {
                            scriptStart.neighbors.Add(scriptEnd);
                        }
                    }
                    else { Debug.Log($"Không tìm được Node ở Object: {y}"); }

                });
            }
            else { Debug.Log($"Không tìm được Node ở Object: {x}"); }
        });
    }    
}
