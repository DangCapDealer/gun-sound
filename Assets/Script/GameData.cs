using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;



#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class GameData : MonoBehaviour
{
    
#if UNITY_EDITOR && EDITOR_LOAD_GUNS
    [SerializeField] private Sprite[] allOfGuns;
    public GameObject gunItemPrefab;
    public Transform gunItemParent;
#endif

    [ContextMenu("Editor Load All Guns")]
    private void EditorLoadAllGuns()
    {
        Debug.Log("[GameData] Editor Load All Guns");
#if UNITY_EDITOR && EDITOR_LOAD_GUNS
        allOfGuns = SpriteSheetLoader.LoadLargeSprites("Assets/Texture2D/gun.png", 32, 32);
        EditorUtility.SetDirty(this);
        EditorSceneManager.MarkSceneDirty(gameObject.scene);
        EditorSceneManager.SaveOpenScenes();
#endif    
    }

    [ContextMenu("Created All Guns")]
    private void CreatedAllGuns()
    {
#if UNITY_EDITOR && EDITOR_LOAD_GUNS
        Debug.Log("[GameData] Created All Guns: " + allOfGuns.Length);
        var lenght = gunItemParent.childCount;
        for (int i = lenght - 1; i >= 0; i--)
        {
            var child = gunItemParent.GetChild(i);
            DestroyImmediate(child.gameObject);
        }

        for(int i = 0; i < allOfGuns.Length; i++)
        {
            var gunName = allOfGuns[i].name;
            var gunSprite = allOfGuns[i];
            var gunItem = PrefabUtility.InstantiatePrefab(gunItemPrefab, gunItemParent) as GameObject;
            gunItem.name = $"Gun_{i}_{gunSprite.name}";
            var gun_img = gunItem.GetChild(1).GetChild(0).GetComponent<Image>();
            gun_img.sprite = gunSprite;
        }
#endif
        
    }
}
