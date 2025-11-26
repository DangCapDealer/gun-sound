// AUTO-GENERATED from AdManager.Native.Collap.cs with safe renames to avoid symbol collisions.
// Behavior mirrors the overlay version but uses independent fields & methods (NativeFull*).

#if ADMOB
using DG.Tweening;
using JKit.Monetize.Ads;

#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public partial class AdManager
{
#if ADMOB
    [Header("NATIVE FULL AD")]
    public bool IsPreloadNativeFullAd = true;
    public AdState NativeFullAdState = AdState.NotAvailable;
    public int NativeFullAdReloadCount = 0;
    public int NativeFullShowCount = 0;

    private bool NativeFullAdUsed = false;
    private bool NativeFullShowing = false;

    //public float _WillOpenNativeFullAdAfter = 5.0f;
    //private int _NumberOfNativeFull = 0;
    private float _FullAdTimer = 0;
    public float FullAdSpaceTimeCounter = 0.0f;
    public float FullAdSpaceTime = 15.0f;
    public string _adUnitNativeFullId = "ca-app-pub-3940256099942544/2247696110";

    private NativeOverlay _nativeFullAd;
    private GameObject _nativeOverlayAdFullCanvas;
    private GameObject _nativeOverlayAdLoadingCanvas;


    private void PromissShowNativeFull()
    {
        IsCacheNativeFullScreen = true;
        _FullAdTimer = 0.0f;
    }

    private void CaculaterCounterFullAd()
    {
        if (RuntimeStorageData.CanLoadAd() == false) return;
        if (Manager.Instance.IsLoading == true) return;


        if (IsCacheNativeFullScreen == true)
        {
            _FullAdTimer += Time.deltaTime;
            if (_FullAdTimer >= 10.0f)
            {
                _FullAdTimer = 0;
                IsCacheNativeFullScreen = false;
                PromisseShowNativeFullAdWithLoading();
            }
        }

        if (NativeFullShowing == true)
        {
            FullAdSpaceTimeCounter += Time.deltaTime;
            if (FullAdSpaceTimeCounter >= 30.0f)
            {
                FullAdSpaceTimeCounter = 0;
                LoadNativeFullAd();
            }
        }
    }
    
    public void LoadNativeFullAd()
    {
        try
        {
            if (RuntimeStorageData.CanLoadAd() == false) return;
            if (NativeFullAdState == AdState.Loading) return;
            NativeFullAdState = AdState.Loading;
    
            if (_nativeFullAd != null)
            {
                Debug.Log($"[{this.GetType().ToString()}] Destroying existing Native Overlay ad before loading a new one.");
                _nativeFullAd.Destroy();
                _nativeFullAd = null;
            }
    
            NativeOverlay.Load(_adUnitNativeFullId, 30, (overlay, error) =>
            {
                try
                {
                    if (error != null)
                    {
                        NativeFullAdState = AdState.NotAvailable;
                    }
                    else
                    {
                        NativeFullAdState = AdState.Ready;
                        _nativeFullAd = overlay;
                        _nativeFullAd.OnClosed += onNativeFullAdClosed;
                        _nativeFullAd.OnClicked += onNativeFullAdClicked;
                    }
                }
                catch (System.Exception ex)
                {
                    LogSystem.LogError($"[AdManager] Exception in NativeOverlay.Load callback: {ex}");
                    NativeFullAdState = AdState.NotAvailable;
                }
            });
        }
        catch (System.Exception ex)
        {
            LogSystem.LogError($"[AdManager] Exception in LoadNativeFullAd: {ex}");
            NativeFullAdState = AdState.NotAvailable;
        }
    }

    private void onNativeFullAdClosed()
    {
        ShowBanner();
        NativeFullAdState = AdState.NotAvailable;
        NativeFullShowing  = false;
    }

    private void onNativeFullAdClicked()
    {
        TaskRunner.EnqueueTask(LoadNativeFullAd);
    }

    public void PromisseShowNativeFullAdWithLoading()
    {
        if (RuntimeStorageData.CanLoadAd() == false) return;
        if (_nativeFullAd != null)
        {
            Debug.Log($"[{this.GetType().ToString()}] Promisse Showing Native Overlay ad.");
            DOVirtual.DelayedCall(1.0f, ShowNativeFullAd);
        }
    }
        
    public void ShowNativeFullAd()
    {
        if (RuntimeStorageData.CanLoadAd() == false) return;
        if (_nativeFullAd != null)
        {
            Debug.Log($"[{this.GetType().ToString()}] Showing Native Overlay ad.");
            NativeFullShowCount += 1;
            DOVirtual.DelayedCall(0.1f, _nativeFullAd.Show);
            HideBanner();
            NativeOverlayShowing = true;
        }
    }
#else
    public void ShowAdNativeFull() { }
#endif
}