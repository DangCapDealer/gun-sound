using System;
using System.Collections.Generic;

public static class EventBus
{
    private static readonly Dictionary<Type, Delegate> events = new();

    public static void Subscribe<T>(Action<T> callback)
    {
        if (events.TryGetValue(typeof(T), out var del))
            events[typeof(T)] = Delegate.Combine(del, callback);
        else
            events[typeof(T)] = callback;
    }

    public static void Unsubscribe<T>(Action<T> callback)
    {
        if (events.TryGetValue(typeof(T), out var del))
        {
            var currentDel = Delegate.Remove(del, callback);
            if (currentDel == null) events.Remove(typeof(T));
            else events[typeof(T)] = currentDel;
        }
    }

    public static void Publish<T>(T evt)
    {
        if (events.TryGetValue(typeof(T), out var del))
            (del as Action<T>)?.Invoke(evt);
    }
}

public static class EventBusExtensions
{
    private static readonly CanvasEvent _canvasEvent = new CanvasEvent();
    public static CanvasEvent CanvasEvent(this string canvasId)
    {
        _canvasEvent.CanvasId = canvasId;
        return _canvasEvent;
    }
}

public class CanvasEvent { public string CanvasId; }

/*
-------------------------
CÁCH SỬ DỤNG EventBus
-------------------------

// 1. Định nghĩa một class event (có thể là struct/class)
public class PlayerDiedEvent
{
    public int playerId;
    public PlayerDiedEvent(int id) { playerId = id; }
}

// 2. Đăng ký lắng nghe event ở bất kỳ đâu:
void OnEnable()
{
    EventBus.Subscribe<PlayerDiedEvent>(OnPlayerDied);
}
void OnDisable()
{
    EventBus.Unsubscribe<PlayerDiedEvent>(OnPlayerDied);
}
void OnPlayerDied(PlayerDiedEvent evt)
{
    Debug.Log("Player died: " + evt.playerId);
}

// 3. Gửi event ở bất kỳ đâu:
EventBus.Publish(new PlayerDiedEvent(123));

// 4. Có thể dùng cho bất kỳ kiểu event nào, không cần reference trực tiếp giữa các script.

*/