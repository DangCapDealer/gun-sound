using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public static readonly string HomeId = "home";
    public static readonly string GameId = "game";
    public static readonly string SettingId = "setting";
    public void Start()
    {
        EventBus.Publish(EventBusExtensions.CanvasEvent(GameId));
    }
}
