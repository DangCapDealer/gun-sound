using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Events;
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

    public void OnGameButtonPressed(string gunId)
    {
        Debug.Log("[HomeUICanvas] Gun: " + gunId);
        EventBus.Publish(EventBusExtensions.CanvasEvent(GameManager.GameId));
    }

    public void OnSetingButtonPressed()
    {
        Debug.Log("[HomeUICanvas] Setting Button Pressed");
        EventBus.Publish(EventBusExtensions.PopupEvent(GameManager.SettingId));
    }

    [ContextMenu("Setting Name Of Gun")]
    private void SettingNameOfGun()
    {
#if UNITY_EDITOR
        int _contentCount = _content.childCount;
        for (int i = 0; i < _contentCount; i++)
        {
            var child = _content.GetChild(i);
            var gunId = child.name.Split('_')[1];
            var gun_txt = child.GetChild(2).GetChild(0).GetComponent<UnityEngine.UI.Text>();
            gun_txt.text = gunId;

            var btn = child.GetComponent<UnityEngine.UI.Button>();
            if (btn == null)
            {
                btn = child.gameObject.AddComponent<UnityEngine.UI.Button>();
            }
            // Xóa tất cả persistent listener trỏ tới OnGameButtonPressed
            for (int j = btn.onClick.GetPersistentEventCount() - 1; j >= 0; j--)
            {
                if (btn.onClick.GetPersistentMethodName(j) == nameof(OnGameButtonPressed))
                    UnityEventTools.RemovePersistentListener(btn.onClick, j);
            }
            btn.onClick.RemoveAllListeners();
            UnityEventTools.AddStringPersistentListener(btn.onClick, OnGameButtonPressed, gunId);
            // Đánh dấu từng GameObject con đã thay đổi
            EditorUtility.SetDirty(child.gameObject);
        }
        // Đánh dấu scene đã thay đổi và lưu lại
        EditorSceneManager.MarkSceneDirty(gameObject.scene);
        EditorSceneManager.SaveOpenScenes();
#endif   
    }
}
