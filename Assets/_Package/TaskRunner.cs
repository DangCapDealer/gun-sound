using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quản lý và thực thi các nhiệm vụ trên luồng chính (main thread) của Unity
/// khi hàm Update() đang hoạt động.
/// </summary>
public class TaskRunner : MonoSingletonGlobal<TaskRunner>
{
    // Sử dụng Queue để đảm bảo các nhiệm vụ được thực thi theo thứ tự.
    private static readonly Queue<Action> _taskQueue = new Queue<Action>();
    private static bool _quitting = false; // Cờ để xử lý khi ứng dụng đang thoát

    private void OnApplicationQuit()
    {
        _quitting = true; // Đặt cờ khi ứng dụng chuẩn bị thoát
    }

    // --- CÁCH HOẠT ĐỘNG CHÍNH ---
    private void Update()
    {
        // Kiểm tra xem có nhiệm vụ nào trong hàng đợi không
        if (_taskQueue.Count > 0)
        {
            // Sử dụng lock để đảm bảo an toàn luồng khi truy cập Queue
            // (Đặc biệt quan trọng nếu bạn enqueue từ luồng khác)
            lock (_taskQueue)
            {
                // Lấy ra và thực thi từng nhiệm vụ một cho đến khi hàng đợi trống
                while (_taskQueue.Count > 0)
                {
                    Action task = _taskQueue.Dequeue();
                    try
                    {
                        task?.Invoke(); // Thực thi nhiệm vụ
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Error executing task on main thread: {ex.Message}\n{ex.StackTrace}");
                    }
                }
            }
        }
    }

    /// <summary>
    /// Thêm một nhiệm vụ vào hàng đợi để được thực thi trên luồng chính trong hàm Update tiếp theo.
    /// </summary>
    /// <param name="action">Hành động (phương thức) bạn muốn thực thi.</param>
    public static void EnqueueTask(Action action)
    {
        if (_quitting)
        {
            Debug.LogWarning("Application is quitting, cannot enqueue new tasks.");
            return;
        }
        if (action == null)
        {
            Debug.LogWarning("Attempted to enqueue a null action.");
            return;
        }

        // Đảm bảo TaskRunner đã được khởi tạo trước khi thêm task
        if (Instance == null)
        {
            Debug.LogError("TaskRunner instance is null. Make sure it's in your scene or created correctly.");
            return;
        }

        lock (_taskQueue)
        {
            _taskQueue.Enqueue(action);
        }
    }
}