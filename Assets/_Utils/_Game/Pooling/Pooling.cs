using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class Pooling
{
    private static Pooling _instance;
    public static Pooling I => _instance ??= new Pooling();

    private readonly Dictionary<int, List<GameObject>> pools = new();
    private readonly Dictionary<string, GameObject> poolPrefabs = new();

    private Pooling() { }

    public GameObject GetPrefab(string path)
    {
        if (!poolPrefabs.TryGetValue(path, out var prefab))
        {
            prefab = Resources.Load<GameObject>(path);
            if (prefab == null)
            {
                Debug.LogError($"PoolByID: Prefab not found at path: {path}");
                return null;
            }
            poolPrefabs[path] = prefab;
        }
        return GetPrefab(prefab);
    }

    public GameObject GetPrefab(GameObject prefab)
    {
        int id = prefab.GetInstanceID();
        if (!pools.TryGetValue(id, out var list))
            pools[id] = list = new List<GameObject>();

        foreach (var go in list)
            if (!go.activeSelf)
                return Activate(go);

        var obj = Object.Instantiate(prefab);
        obj.name = prefab.name;
        list.Add(obj);
        return Activate(obj);
    }

    public GameObject GetPrefab(GameObject prefab, Vector3 pos, Quaternion rot, Transform parent = null)
    {
        int id = prefab.GetInstanceID();
        if (!pools.TryGetValue(id, out var list))
            pools[id] = list = new List<GameObject>();

        foreach (var go in list)
            if (!go.activeSelf)
                return Activate(go, pos, rot, parent);

        var obj = Object.Instantiate(prefab, pos, rot, parent);
        obj.name = prefab.name;
        list.Add(obj);
        return Activate(obj, pos, rot, parent);
    }

    public GameObject GetPrefab(GameObject prefab, Transform parent)
        => GetPrefab(prefab, prefab.transform.position, prefab.transform.rotation, parent);

    public T GetPrefab<T>(GameObject prefab) where T : Component
        => GetPrefab(prefab)?.GetComponent<T>();

    public T GetPrefab<T>(GameObject prefab, Vector3 pos, Quaternion rot) where T : Component
        => GetPrefab(prefab, pos, rot)?.GetComponent<T>();

    public T GetPrefab<T>(GameObject prefab, Transform parent) where T : Component
        => GetPrefab(prefab, parent)?.GetComponent<T>();

    public void PushToPool(GameObject obj, bool keepParent = false)
    {
        if (obj == null) return;
        if (!keepParent) obj.transform.SetParent(null);
        obj.SetActive(false);
    }

    public void PushToPool(GameObject obj, float time)
    {
        if (obj == null) return;
        DOVirtual.DelayedCall(time, () => PushToPool(obj));
    }

    private GameObject Activate(GameObject go)
    {
        go.SetActive(true);
        return go;
    }
    private GameObject Activate(GameObject go, Vector3 pos, Quaternion rot, Transform parent)
    {
        go.transform.SetParent(parent);
        go.transform.SetPositionAndRotation(pos, rot);
        go.SetActive(true);
        return go;
    }
}