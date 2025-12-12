using UnityEngine;

public class GameUICanvas : ScreenCanvas
{
    public void OnBackButtonPressed()
    {
        EventBus.Publish(EventBusExtensions.CanvasEvent(GameManager.HomeId));
    }

    public void OnSettingButtonPressed()
    {
        EventBus.Publish(EventBusExtensions.PopupEvent(GameManager.SettingId));
    }
}
