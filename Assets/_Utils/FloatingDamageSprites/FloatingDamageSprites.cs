using UnityEngine;
using System.Collections.Generic;
using EditorCools;

public class FloatingDamageSprites : MonoBehaviour
{
    public FloatingDamageInstance damageInstancePrefab;
    private readonly Queue<FloatingDamageInstance> pool = new();

    // Gọi hàm này để hiển thị damage
    public void DisplayDamage(int damage, Vector3 pos, bool crit)
    {
        FloatingDamageInstance inst = null;
        while (pool.Count > 0)
        {
            var candidate = pool.Dequeue();
            if (!candidate.gameObject.activeSelf)
            {
                inst = candidate;
                break;
            }
        }
        if (inst == null)
            inst = Instantiate(damageInstancePrefab, transform);

        inst.gameObject.SetActive(true);
        inst.Show(damage, pos, crit, () => {
            inst.gameObject.SetActive(false);
            pool.Enqueue(inst);
        });
    }
    
    [Button]
    private void Display()
    {
        // Test hiển thị damage
        DisplayDamage(Random.Range(500, 1000000000), new Vector3(0, 0, 0), Random.value > 0.7f);
    }
}