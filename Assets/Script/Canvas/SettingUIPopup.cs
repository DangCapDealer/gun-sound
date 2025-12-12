using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SettingUIPopup : PopupCanvas
{
    [Header("Content Transform")]
    [SerializeField] private Transform _contentTransform;
    [Header("Toggles")]
    [SerializeField] private Toggle hapticToggle;
    [SerializeField] private Toggle flashToggle;
    [SerializeField] private Toggle effectToggle;
    [SerializeField] private Toggle infinityToggle;
    protected override void Initialized(UnityAction _actionFirst, UnityAction _actionSecond, string[] msg)
    {
        ModifyRotation();

        // Đồng bộ trạng thái Toggle với Setting
        hapticToggle.isOn = RuntimeStorageData.Setting.Get("vibrate", true);
        flashToggle.isOn = RuntimeStorageData.Setting.Get("flash", false);
        effectToggle.isOn = RuntimeStorageData.Setting.Get("effect", true);
        infinityToggle.isOn = RuntimeStorageData.Setting.Get("infinity_ammo", false);

        Debug.Log("Setting UI Initialized with values - Vibrate: " + hapticToggle.isOn +
            ", Flash: " + flashToggle.isOn +
            ", Effect: " + effectToggle.isOn +
            ", Infinity Ammo: " + infinityToggle.isOn);
    }

    public void OnCloseButtonPressed()
    {
        EventBus.Publish(EventBusExtensions.PopupEvent(""));
    }

    public void OnHapticToggleChanged()
    {
        RuntimeStorageData.Setting.Set("vibrate", hapticToggle.isOn);
        Debug.Log("Vibrate setting changed to: " + hapticToggle.isOn);
    }

    public void OnFlashToggleChange()
    {
        RuntimeStorageData.Setting.Set("flash", flashToggle.isOn);
        Debug.Log("Flash setting changed to: " + flashToggle.isOn);
    }

    public void OnEffectToggleChange()
    {
        RuntimeStorageData.Setting.Set("effect", effectToggle.isOn);
        Debug.Log("Effect setting changed to: " + effectToggle.isOn);
    }

    public void OnInfinityToggleChanged()
    {
        RuntimeStorageData.Setting.Set("infinity_ammo", infinityToggle.isOn);
        Debug.Log("Infinity Ammo setting changed to: " + infinityToggle.isOn);
    }

    private void ModifyRotation()
    {
        if (CanvasManager.I.CanvasId == GameManager.HomeId) _contentTransform.localRotation = _contentTransform.localRotation.WithZ(0f);
        else if (CanvasManager.I.CanvasId == GameManager.GameId) _contentTransform.localRotation = _contentTransform.localRotation.WithZ(-90.0f);
        else _contentTransform.localRotation = _contentTransform.localRotation.WithZ(0f);
    }
}
