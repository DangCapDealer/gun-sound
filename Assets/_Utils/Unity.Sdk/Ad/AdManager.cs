using System;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using System.Collections;
using JetBrains.Annotations;

#if ADMOB
using GoogleMobileAds.Api;
#endif

[DefaultExecutionOrder(-5)]
public partial class AdManager : SingletonGlobal<AdManager>
{
#if ADMOB
    [Header("SETTING")]
    public bool IsNativeAd = false;
    public bool IsAdShowing = false;
    public bool IsCacheNativeFullScreen = false;

    [Header("AD: OUT OF TIME")]
    public float InterOpenAdMaximusTimeCounter = 0.0f;
    public float InterOpenAdMaximusTime = 15.0f;
    private void CaculaterCounterInterOpenAd()
    {
        if (Manager.Instance.IsLoadAdPromise == false) return;

        if (InterOpenAdMaximusTimeCounter <= InterOpenAdMaximusTime)
        {
            InterOpenAdMaximusTimeCounter += Time.unscaledDeltaTime;
            if (InterOpenAdMaximusTimeCounter > InterOpenAdMaximusTime)
            {
                Manager.Instance.CompleteAd();
            }
        }
    }

    public enum AdState
    {
        Loading, Ready, NotAvailable
    }

    public enum AdBannerSize
    {
        Banner,
        FullWidth
    }

    public enum AdShowState
    {
        None,
        Pending
    }

    [Header("AD STATUS")]

    public bool IsInitalized = false;
    //public bool IsBannerShow = false;

    [Header("AD MANAGER")]
    public string _adUnitId = "ca-app-pub-8190506959251235~5137518209";

    void Start()
    {
        StartCoroutine(Bacon.UMP.Instance.DOGatherConsent(LoadAds()));
    }

    private void OnEnable()
    {
        GameEvent.OnIAPurchase += onInappPurchase;
    }

    private void OnDisable()
    {
        GameEvent.OnIAPurchase -= onInappPurchase;
    }

    private void onInappPurchase(string _productID)
    {
        if (_productID == "subscribe_remove_ad") HideBanner();
    }
        

    private void onShowAd()
    {
        Debug.Log("[AdManager] set timeScale to 0");
        Time.timeScale = 0;
    }

    private void onCloseAd()
    {
        Debug.Log("[AdManager] set timeScale to 1");
        Time.timeScale = 1;
    }

    private IEnumerator LoadAds()
    {
        yield return new WaitUntil(() => Bacon.UMP.Instance.IsUMPReady);
        MobileAds.SetiOSAppPauseOnBackground(true);
        MobileAds.RaiseAdEventsOnUnityMainThread = true;
        MobileAds.Initialize(initStatus =>
        {
            Debug.Log($"[{this.GetType().ToString()}] Admob Initialized");
            IsInitalized = true;
            if (RuntimeStorageData.CanLoadAd() == true)
            {
                //PromiseShowOpenFirstAd();
                //LoadInterstitialOpenAd();
                PromiseShowInterstitialOpenAd();
            }
            else Manager.Instance.CompleteAd();
        });
        yield return new WaitUntil(() => IsInitalized);
#if UNITY_EDITOR
        yield return WaitForSecondCache.WAIT_TIME_FIVE;
#endif
        LoadRewardedAd();
        LoadInterstitialAd();
        LoadAppOpenAd();
        LoadNativeOverlayAd();
        LoadNativeFullAd();
        LoadBannerAd();

    }

    private float AfterAdReload = 10.0f;

    // Gộp reload timer thành một biến duy nhất
    private float TimerAdReload = 0f;

    private float timerLogUMP = 0;

    private void Update()
    {
        if (IsInitalized == false) return;


        TimerAdReload += Time.deltaTime;
        if (TimerAdReload > AfterAdReload)
        {
            TimerAdReload = 0;

            // Lần lượt kiểm tra từng ad, cái nào NotAvailable thì load
            if (IsPreloadInterstitial && InterAdState == AdState.NotAvailable && RuntimeStorageData.CanLoadAd() == true)
                LoadInterstitialAd();

            if (IsPreloadReward && RewardAdState == AdState.NotAvailable)
                LoadRewardedAd();

            if (IsPreloadOpen && OpenAdState == AdState.NotAvailable && RuntimeStorageData.CanLoadAd() == true)
                LoadAppOpenAd();

            if (IsPreloadNativeOverlayAd && NativeOverlayAdState == AdState.NotAvailable && RuntimeStorageData.CanLoadAd() == true)
                LoadNativeOverlayAd();

            if (IsPreloadNativeFullAd && NativeFullAdState == AdState.NotAvailable && RuntimeStorageData.CanLoadAd() == true)
                LoadNativeFullAd();
        }

        if (IsPreloadBanner && BannerAdState == AdState.NotAvailable && RuntimeStorageData.CanLoadAd() == true)
            LoadBannerAd();

        if (RuntimeStorageData.CanLoadAd() == false) return;

        CaculaterCounterInterAd();
        CaculaterCounterInterOpenAd();
        CaculaterCounterCollapseAd();
        CaculaterCounterOpenAd();
        CaculaterCounterFullAd();
        CaculaterCounterBannerAd();
        //PromiessShowInterstitialOpenAfterLoaded();

    }

    public void HideBannerAd() { }
    public void ShowBannerAd() { }
    public bool IsCanAutoAd() { return !IsNativeAd; }
#else
    public bool IsNativeAd = false;
    public float OpenAdSpaceTimeCounter = 0.0f;

    private void Start()
    {
        DOVirtual.DelayedCall(5.0f, () => Concurrency.EnqueueTask(() => Manager.Instance.CompletePromisseAd()));
    }

    public void ShowInterstitialFreeAd(Action action_1, Action action_2)
    {
        Concurrency.EnqueueTask(action_1);
        Concurrency.EnqueueTask(action_2);
    }

    public void ShowInterstitialOutSpaceTime(Action action)
    {
        Concurrency.EnqueueTask(action);
    }

    public void ShowRewardedAd(UnityAction CALLBACK) { CALLBACK?.Invoke(); }

    public void ShowInterstitialAdWithSpaceTime(UnityAction CALLBACK) { CALLBACK?.Invoke(); }

    public void ShowInterstitialAd(UnityAction CALLBACK) { CALLBACK?.Invoke(); }

    public void CheckingOpenAd() { }

    public void ShowBannerAd() { }

    public void ShowRewardedHintAd(UnityAction CALLBACK) { CALLBACK?.Invoke(); }

    public void ShowInterstitialForcedAd(UnityAction CALLBACK) { CALLBACK?.Invoke(); }

    public void ShowRewardedForceAd(UnityAction CALLBACK) { CALLBACK?.Invoke(); }

    public void HideBannerAd() { }

    public void CanShowInterstitialOpen() { }

    public void ShowBanner() { }
    public void HideBanner() { }
    public void HideNativeFullAd() { }
    public void ShowNativeFullAd() { }
    public void ShowRewardedAd(UnityAction CALLBACK_EVENT, string placement_id) { CALLBACK_EVENT?.Invoke(); }
#endif
}
