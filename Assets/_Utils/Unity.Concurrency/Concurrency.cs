using System;
using System.Collections.Generic;
using UnityEngine;

public class Concurrency : SingletonGlobal<Concurrency>
{
    private static readonly Queue<Action> _taskQueue = new();
    private static bool _quitting = false;

    private void OnApplicationQuit() => _quitting = true;

    private void Update()
    {
        lock (_taskQueue)
        {
            while (_taskQueue.Count > 0)
            {
                try { _taskQueue.Dequeue()?.Invoke(); }
                catch (Exception ex) { Debug.LogError($"TaskRunner error: {ex}"); }
            }
        }
    }

    public static void EnqueueTask(Action action)
    {
        if (_quitting || action == null || Instance == null) return;
        lock (_taskQueue) { _taskQueue.Enqueue(action); }
    }
}