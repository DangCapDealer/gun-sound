using DG.Tweening;
using EditorCools;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public enum Scene
{
    Loading,
    Ingame,
}

[DefaultExecutionOrder(-10)]
public class Manager : MonoSingletonGlobal<Manager>
{
    [Header("Loading")]
    public bool IsFirebase = false;
    public bool IsIngame = false;
    public bool IsLoading = false;


    public bool IsPauseFromAd() => AdManager.Instance.IsNativeAd;
    public bool IsLoadAdPromise = true;


    [SerializeField] private LoadingCanvas loadingCanvas;

    private bool isCompleteAdCallback = false;  
    public event Action CompleteAdCallback;


    [Button]
    private void AddCoin()
    {
        RuntimeStorageData.Player.Gold += 10000;
    }

    protected override void Awake()
    {
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60; 

        base.Awake();

        RuntimeStorageData.LoadAllData();
        if(RuntimeStorageData.Player.LastDayLogin != DateTime.Now.Day)
        {
            RuntimeStorageData.Player.LastDayLogin = DateTime.Now.Day;
            RuntimeStorageData.Player.NumberOfDay += 1;
        }
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
        Time.timeScale = 1;
        LoadingAd();
    }

    private void LoadingAd()
    {
        IsLoading = true;
        loadingCanvas.Show(null, 0.0f, 0.9f);
    }    

    public void CompleteAd()
    {
        if (isCompleteAdCallback == true) return;
        isCompleteAdCallback = true;

        IsLoading = false;
        IsLoadAdPromise = false;

        loadingCanvas.Hide();
        CompleteAdCallback?.Invoke();
    }     

    public LoadingCanvas LoadingUI() => loadingCanvas;
 
    
    public void HideLoading()
    {
        DOVirtual.DelayedCall(1.0f, () => loadingCanvas.Hide());
    }

    public int NumberOfPause = 0;
    public void OnApplicationPause(bool pause)
    {
        // Debug.Log($"Application Pause {pause}");
        RuntimeStorageData.SaveAllData();

        NumberOfPause += 1;
        if (NumberOfPause <= 2)
            return;

        if (pause == false)
            AdManager.Instance.CheckingOpenAd();
    }

    public void OnApplicationQuit()
    {
        RuntimeStorageData.SaveAllData();
    }
}
