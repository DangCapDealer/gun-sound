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
    public bool IsFirebase = false;
    public bool IsAd = false;
    public bool IsLoading = false;

    [Header("References")]
    [SerializeField] private LoadingCanvas loadingCanvas;

    private bool isCompleteAdCallback = false;
    public event Action CompleteAdCallback;


    [Header("Application State")]
    public int NumberOfPause = 0;

    protected override void Awake()
    {
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;
        base.Awake();

        RuntimeStorageData.LoadAllData();
        if (RuntimeStorageData.Player.LastDayLogin != DateTime.Now.Day)
        {
            RuntimeStorageData.Player.LastDayLogin = DateTime.Now.Day;
            RuntimeStorageData.Player.NumberOfDay += 1;
        }
    }

    private void Start()
    {
        Time.timeScale = 1;
        ShowLoading();
    }

    private void ShowLoading()
    {
        IsLoading = true;
        loadingCanvas.Show(null, 0.0f, 0.9f);
    }

    public void HideLoading(float delay = 1.0f)
    {
        IsLoading = false;
        loadingCanvas.Hide(delay);
    }

    public LoadingCanvas LoadingUI() => loadingCanvas;

    public void CompletePromisseAd()
    {
        if (isCompleteAdCallback) return;
        isCompleteAdCallback = true;
        HideLoading(0.0f);
        CompleteAdCallback?.Invoke();
    }

    private void OnApplicationPause(bool pause)
    {
        RuntimeStorageData.SaveAllData();
        NumberOfPause++;
        if (NumberOfPause <= 2) return;
        if (!pause)
            AdManager.Instance.CheckingOpenAd();
    }

    private void OnApplicationQuit()
    {
        RuntimeStorageData.SaveAllData();
    }
}
