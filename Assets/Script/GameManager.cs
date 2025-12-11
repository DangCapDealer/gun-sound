using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public static readonly string HomeId = "home";
    public static readonly string GameId = "game";
    public void Start()
    {
        EventBus.Publish(EventBusExtensions.CanvasEvent(GameId));
    }
}
