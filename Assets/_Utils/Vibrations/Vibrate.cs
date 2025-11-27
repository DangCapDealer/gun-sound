using MoreMountains.NiceVibrations;
using UnityEngine;

public class Vibrate : SingletonNonMono<Vibrate>
{
    public bool IsVibrationEnabled = false;

    public void PlayHapticPulse(float intensity, float sharpness)
    {
        Debug.Log($"[Vibrate] PlayHapticPulse intensity:{intensity} sharpness:{sharpness}");
        if (!IsVibrationEnabled) return;
        if (RuntimeStorageData.Sound.isVibrate)
            MMVibrationManager.TransientHaptic(intensity, sharpness, true);
    }

    public void PlayHapticType(HapticTypes type)
    {
        Debug.Log($"[Vibrate] PlayHapticType type:{type}");
        if (!IsVibrationEnabled) return;
        if (RuntimeStorageData.Sound.isVibrate)
            MMVibrationManager.Haptic(type, false, true);
    }

    public void PlayHapticContinuous(float intensity, float sharpness, float duration)
    {
        Debug.Log($"[Vibrate] PlayHapticContinuous intensity:{intensity} sharpness:{sharpness} duration:{duration}");
        if (!IsVibrationEnabled) return;
        if (RuntimeStorageData.Sound.isVibrate)
            MMVibrationManager.ContinuousHaptic(intensity, sharpness, duration);
    }

    public void PlaySelectionClick()
    {
        Debug.Log("[Vibrate] PlaySelectionClick");
        if (!IsVibrationEnabled) return;
        if (RuntimeStorageData.Sound.isVibrate)
            MMVibrationManager.Haptic(HapticTypes.Selection, false, true);
    }
}
