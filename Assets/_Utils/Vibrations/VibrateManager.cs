using MoreMountains.NiceVibrations;
using UnityEngine;

public class VibrateManager : Singleton<VibrateManager>
{
    public bool IsVibrate = false;

    public void PlayVibrate(float intensity, float sharpness)
    {
        if (IsVibrate == false) return;

        if (RuntimeStorageData.Sound.isVibrate)
            MMVibrationManager.TransientHaptic(intensity, sharpness, true, this);
    }    

    public void PlayVibrate(HapticTypes type)
    {
        if (IsVibrate == false) return;

        if (RuntimeStorageData.Sound.isVibrate)
            MMVibrationManager.Haptic(type, false, true, this);
    }    

    public void PlayVibrate(float intensity, float sharpness, float duration)
    {
        if (IsVibrate == false) return;

        if (RuntimeStorageData.Sound.isVibrate)
            MMVibrationManager.ContinuousHaptic(intensity, sharpness, duration);
    }    

    public void PlayClick()
    {
        if (IsVibrate == false) return;

        if (RuntimeStorageData.Sound.isVibrate)
            MMVibrationManager.Haptic(HapticTypes.Selection, false, true, this);
    }    
}
