using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using JetBrains.Annotations;
using Cysharp.Threading.Tasks;
#if FIREBASE
using Firebase.Extensions;
using Firebase.Analytics;
#endif

public class FirebaseManager : CSharpSingleton<FirebaseManager>
{
#if FIREBASE
    public bool IsInitialized = false;
    Firebase.FirebaseApp app;

    public async UniTask InitializeAsync()
    {
        Firebase.FirebaseApp.Create();
        await UniTask.Yield();

        var dependencyTask = Firebase.FirebaseApp.CheckAndFixDependenciesAsync();
        await UniTask.WaitUntil(() => dependencyTask.IsCompleted);

        FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
        var dependencyStatus = dependencyTask.Result;
        if (dependencyStatus == Firebase.DependencyStatus.Available)
        {
            app = Firebase.FirebaseApp.DefaultInstance;
            IsInitialized = true;
            RunAllMessage();
        }
    }

    public void TRACKING_RATIO()
    {
        float ratio = (float)Screen.height / (float)Screen.width;
        string commonRatio = "Unknown";

        if (ratio >= 2.3f) commonRatio = "21:9";
        else if (ratio >= 2.2f) commonRatio = "20:9";
        else if (ratio >= 2.1f) commonRatio = "19:9";
        else if (ratio >= 2.0f) commonRatio = "18:9";
        else if (ratio >= 1.7f) commonRatio = "16:9";
        else if (ratio >= 1.6f) commonRatio = "16:10";
        else if (ratio >= 1.5f) commonRatio = "3:2";
        else if (ratio >= 1.3f) commonRatio = "4:3";
        else if (ratio >= 1.2f) commonRatio = "5:4";
        else if (ratio >= 1.0f) commonRatio = "1:1";

        if (!IsInitialized)
            AddMessage(TrackingRatio);
        else
            TrackingRatio();

        void TrackingRatio()
        {
            LogSystem.LogSuccess($"FIREBASE TRACKING_RATIO --> Ratio: {commonRatio}");
            LogMessage($"tracking_ratio_{commonRatio.Replace(":", "_")}");
        }
    }

    public void TRACKING_ADS_EVENT(string eventName, string placement_id, string value)
    {
        if (!IsInitialized)
            AddMessage(TrackingAdEvent);
        else
            TrackingAdEvent();

        void TrackingAdEvent()
        {
            LogSystem.LogSuccess($"FIREBASE TRACKING_ADS_EVENT --> {eventName} | placement_id: {placement_id} | value: {value}");
            LogMessage(eventName, new[]
            {
                new Parameter("placement_id", placement_id),
                new Parameter("value", value)
            });
        }
    }
    
    public void TRACKING_EVENT(string eventName)
    {
        if (!IsInitialized)
            AddMessage(TrackingEvent);
        else
            TrackingEvent();

        void TrackingEvent()
        {
            LogSystem.LogSuccess($"FIREBASE TRACKING_EVENT --> {eventName}");
            LogMessage(eventName);
        }
    }

    public void TRACKING_EVENT(string eventName, params (string key, object value)[] parameters)
    {
        if (!IsInitialized)
            AddMessage(TrackingEvent);
        else
            TrackingEvent();

        void TrackingEvent()
        {
            var paramArr = BuildParameters(parameters);
            string paramLog = parameters != null && parameters.Length > 0
                ? string.Join(", ", parameters.Select(p => $"{p.key}:{p.value?.ToString() ?? ""}"))
                : "No Params";
            LogSystem.LogSuccess($"FIREBASE TRACKING_EVENT --> {eventName} | Params: {paramLog}");
            LogMessage(eventName, paramArr);
        }
    }

    private static Parameter[] BuildParameters(params (string key, object value)[] parameters)
    {
        if (parameters == null || parameters.Length == 0)
            return System.Array.Empty<Parameter>();

        var arr = new Parameter[parameters.Length];
        for (int i = 0; i < parameters.Length; i++)
        {
            var item = parameters[i];
            arr[i] = new Parameter(item.key, item.value?.ToString() ?? "");
        }
        return arr;
    }

    public void LOG_UMP(string msg)
    {
        if (!IsInitialized)
            AddMessage(LogUMP);
        else
            LogUMP();

        void LogUMP()
        {
            LogMessage(msg);
        }
    }

    public void LOG_EVENT(string msg)
    {
        if (!IsInitialized)
            AddMessage(LogEvent);
        else
            LogEvent();

        void LogEvent()
        {
            LogMessage(msg);
        }
    }

    public void LOG_LEVEL_START(params string[] args)
    {
        if (!IsInitialized)
            AddMessage(LogLevelStart);
        else
            LogLevelStart();

        void LogLevelStart()
        {
            LogMessage("level_start", new[] { new Parameter("level", args[0]) });
        }
    }

    public void LOG_LEVEL_END(params string[] args)
    {
        if (!IsInitialized)
            AddMessage(LogLevelEnd);
        else
            LogLevelEnd();

        void LogLevelEnd()
        {
            LogMessage("level_end", new[]
            {
                new Parameter("level", args[0]),
                new Parameter("result", args[1])
            });
        }
    }

    private void LogMessage(string message)
    {
        LogSystem.LogSuccess($"FIREBASE LOG --> {message}");
        FirebaseAnalytics.LogEvent(message);
    }

    private void LogMessage(string message, Parameter[] parameters)
    {
        LogSystem.LogSuccess($"FIREBASE LOG --> {message}");
        FirebaseAnalytics.LogEvent(message, parameters);
    }

    private readonly List<System.Action> Messages = new List<System.Action>(8);
    public void AddMessage(System.Action taskAction) => Messages.Add(taskAction);

    private void RunAllMessage()
    {
        foreach (var action in Messages)
            action?.Invoke();
        Messages.Clear();
    }
#endif
}