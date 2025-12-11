using UnityEngine;
#if UNITY_EDITOR && EDITOR_LOAD_GUNS
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class HomeUICanvas : ScreenCanvas
{
    [SerializeField] private string sortBy = "Gun";
    [SerializeField] private Transform _content;
    public void Start() => Sort(sortBy);

    public void Sort(string by)
    {
        sortBy = by;
        int _contentCount = _content.childCount;
        for (int i = 0; i < _contentCount; i++)
        {
            var child = _content.GetChild(i);
            child.SetActive(child.name.StartsWith(sortBy));
        }
    }

    public void Gun(string gunName)
    {
        Debug.Log("[HomeUICanvas] Gun: " + gunName);
    }

    [ContextMenu("Setting Name Of Gun")]
    private void SettingNameOfGun()
    {
#if UNITY_EDITOR && EDITOR_LOAD_GUNS
        int _contentCount = _content.childCount;
        for (int i = 0; i < _contentCount; i++)
        {
            var child = _content.GetChild(i);
            var name = child.name.Split('_')[1];
            var gun_txt = child.GetChild(2).GetChild(0).GetComponent<UnityEngine.UI.Text>();
            gun_txt.text = name;

            var btn = child.GetComponent<UnityEngine.UI.Button>();
            if (btn == null)
            {
                btn = child.gameObject.AddComponent<UnityEngine.UI.Button>();
            }

            btn.onClick.RemoveAllListeners();
            UnityEditor.Events.UnityEventTools.AddStringPersistentListener(btn.onClick, Gun, name);

            // Đánh dấu từng GameObject con đã thay đổi
            EditorUtility.SetDirty(child.gameObject);
        }
        // Đánh dấu scene đã thay đổi và lưu lại
        EditorSceneManager.MarkSceneDirty(gameObject.scene);
        EditorSceneManager.SaveOpenScenes();
#endif   
    }
}
