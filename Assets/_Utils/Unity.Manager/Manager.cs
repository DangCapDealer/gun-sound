using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Scene
{
    Loading,
    Ingame,
}

[DefaultExecutionOrder(-10)]
public class Manager : SingletonGlobal<Manager>
{
    [Header("Sdk State")]
    public bool IsFirebaseInitialized = false;
    public bool IsAdvertisementReady = false;
    public bool IsInAppPurchaseInitialized = false;

    [Header("Loading State")]
    public bool IsLoadingActive = false;
    public bool AutoHideLoadingAfterTimeout = true;

    public float LoadingTimeout = 15f;
    public float LoadingTimer = 0f;

    [Header("References")]
    [SerializeField] private LoadingCanvas loadingCanvas;

    private bool hasAdCallbackCompleted = false;
    public event Action OnAdCallbackCompleted;

    [Header("Application State")]
    public int PauseCount = 0;

    protected override void Awake()
    {
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;
        base.Awake();

        RuntimeStorageData.LoadAllData();
        if (RuntimeStorageData.Player.LastLoginDay != DateTime.Now.Day)
        {
            RuntimeStorageData.Player.LastLoginDay = DateTime.Now.Day;
            RuntimeStorageData.Player.TotalLoginDays += 1;
        }
    }

    private void Start()
    {
        Time.timeScale = 1;
        ShowLoading();
    }

    private void Update()
    {
        if (IsLoadingActive && AutoHideLoadingAfterTimeout)
        {
            LoadingTimer += Time.unscaledDeltaTime;
            if (LoadingTimer >= LoadingTimeout)
            {
                HideLoading(0.0f);
            }
        }
    }

    private void ShowLoading()
    {
        IsLoadingActive = true;
        loadingCanvas.Show(null, 0.0f, 0.9f);
    }

    public void HideLoading(float delay = 1.0f)
    {
        IsLoadingActive = false;
        loadingCanvas.Hide(delay);
    }

    public LoadingCanvas LoadingUI() => loadingCanvas;

    public void CompletePromisseAd()
    {
        if (hasAdCallbackCompleted) return;
        hasAdCallbackCompleted = true;
        HideLoading(0.0f);
        OnAdCallbackCompleted?.Invoke();
    }

    private void OnApplicationPause(bool pause)
    {
        RuntimeStorageData.SaveAllData();
        PauseCount++;
        if (PauseCount <= 2) return;
        if (!pause)
            AdManager.Instance.CheckingOpenAd();
    }

    private void OnApplicationQuit()
    {
        RuntimeStorageData.SaveAllData();
    }
}
