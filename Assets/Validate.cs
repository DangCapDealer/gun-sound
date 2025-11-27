using UnityEngine;
using UnityEngine.InputSystem;

public class Validate : MonoBehaviour
{
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            Vibrate.I.PlayHapticPulse(0.5f, 0.5f);

        if (Keyboard.current.numpad1Key.wasPressedThisFrame)
        {
            var prefab = Pooling.I.GetPrefab("Triangle");
        }

        if (Keyboard.current.numpad0Key.wasPressedThisFrame)
        {
            
        }
    }
}
