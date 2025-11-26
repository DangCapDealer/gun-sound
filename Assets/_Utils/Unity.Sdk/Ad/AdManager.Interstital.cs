using DG.Tweening;
#if ADMOB
using GoogleMobileAds.Api;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public partial class AdManager
{
#if ADMOB
    [Header("AD INTERSTITIAL")]
    public bool IsPreloadInterstitial = true;
    public AdState InterAdState = AdState.NotAvailable;

    public UnityAction ActionOnAfterInterstitalAd;

    private InterstitialAd _interstitialAd;

    public AdShowState InterAdShowState = AdShowState.None;

    public int _interstitalReloadCount = 0;
    private GameObject _loadingInterstitialAd;

    public float InterAdSpaceTimeCounter = 0.0f;
    public float InterAdSpaceTime = 15.0f;
    public string _adUnitInterId = "ca-app-pub-5904408074441373/8357882100";

    private void CaculaterCounterInterAd()
    {
        if (RuntimeStorageData.CanLoadAd() == false) return;

        InterAdSpaceTimeCounter += Time.deltaTime;
    }

    public void LoadInterstitialAd()
    {
        if (RuntimeStorageData.CanLoadAd() == false) return;

        if (InterAdState == AdState.Loading) return;
        InterAdState = AdState.Loading;

        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        Debug.Log($"[{this.GetType().ToString()}] Loading the interstitial ad.");

        var adRequest = new AdRequest();
        InterstitialAd.Load(_adUnitInterId, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                InterAdState = AdState.NotAvailable;
                _interstitalReloadCount += 1;
                Debug.Log($"[{this.GetType().ToString()}] interstitial ad failed to load an ad with error : {error}");
                return;
            }

            Debug.Log($"[{this.GetType().ToString()}] Interstitial ad loaded with response :{ad.GetResponseInfo()}");

            _interstitialAd = ad;
            ListenToInterAdEvents();
            InterAdState = AdState.Ready;
            _interstitalReloadCount = 0;
        });
    }

    public string interstitial_placement_id = "";

    public void ShowInterstitialAdWithSpaceTime(UnityAction CALLBACK_EVENT, string placement_id = "")
    {
        void FuncCallback() { CALLBACK_EVENT?.Invoke(); }

        if (RuntimeStorageData.CanLoadAd() == false)
        {
            TaskRunner.EnqueueTask(FuncCallback);
        }
        else
        {
            Debug.Log($"[{this.GetType().ToString()}] Show Interstitial Ad With Space Time");
            if (InterAdSpaceTimeCounter > InterAdSpaceTime)
            {
                this.interstitial_placement_id = placement_id;
                InterAdSpaceTimeCounter = 0;
                ShowInterstitialAd(() =>
                {
                    TaskRunner.EnqueueTask(FuncCallback);
                });
            }
            else
            {
                TaskRunner.EnqueueTask(FuncCallback);
            }
        }
    }

    // public void ShowInterstitialOutSpaceTime(UnityAction CALLBACK_EVENT)
    // {
    //     void FuncCallback() { CALLBACK_EVENT?.Invoke(); }

    //     if (RuntimeStorageData.CanLoadAd() == false)
    //     {
    //         TaskRunner.EnqueueTask(FuncCallback);
    //     }
    //     else
    //     {
    //         Debug.Log($"[{this.GetType().ToString()}] Show Interstitial Ad Out Space Time");
    //         InterAdSpaceTimeCounter = 0;
    //         ShowInterstitialAd(() =>
    //         {
    //             TaskRunner.EnqueueTask(FuncCallback);
    //         });
    //     }
    // }

    private void ShowInterstitialAd(UnityAction CALLBACK_EVENT)
    {
        if (InterAdShowState == AdShowState.Pending) return;
        TaskRunner.EnqueueTask(() =>
        {
            Debug.Log("Show Interstitial Ad");
            if (_interstitialAd != null &&
                _interstitialAd.CanShowAd())
            {
                Debug.Log($"[{this.GetType().ToString()}] Showing interstitial ad.");
                InterAdShowState = AdShowState.Pending;
                _loadingInterstitialAd ??= transform.FindChildByParent("LoadingAdCanvas").gameObject;
                _loadingInterstitialAd?.Show();
                OpenAdSpaceTimeCounter = 0;

                ActionOnAfterInterstitalAd = () =>
                {
                    onCloseAd();
                    TaskRunner.EnqueueTask(() =>
                    {
                        //PromisseShowNativeFullScreenAd
                        PromissShowNativeFull();
                        InterAdShowState = AdShowState.None;
                        CALLBACK_EVENT?.Invoke();
                        _loadingInterstitialAd?.Hide();
                    });

                };

                TaskRunner.EnqueueTask(() =>
                {
                    onShowAd();
                    DOVirtual.DelayedCall(1.0f, () =>
                    {
                        TweenSafeTaskRunner.Instance.ThreshLog("inter_ad");
                        IsAdShowing = true;
                        _interstitialAd.Show();
                    }, true);
                });
            }
            else
            {
                TaskRunner.EnqueueTask(() =>
                {
                    Debug.Log($"[{this.GetType().ToString()}] Interstitial ad is not ready yet.");
                    CALLBACK_EVENT?.Invoke();
                });
            }
        });
    }

    private void ListenToInterAdEvents()
    {
        _interstitialAd.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Interstitial ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));

            FirebaseManager.Instance.TRACKING_ADS_EVENT("paid_ads_interstitial", $"interstitial_{this.interstitial_placement_id}", (adValue.Value / (double)1000000).ToString());
            AppflyerEventSender.Instance.logAdRevenue(adValue);
        };

        _interstitialAd.OnAdImpressionRecorded += () =>
        {
            Debug.Log($"[{this.GetType().ToString()}] Interstitial ad recorded an impression.");
        };

        _interstitialAd.OnAdClicked += () =>
        {
            Debug.Log($"[{this.GetType().ToString()}] Interstitial ad was clicked.");
        };

        _interstitialAd.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log($"[{this.GetType().ToString()}] Interstitial ad full screen content opened.");
        };

        _interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log($"[{this.GetType().ToString()}] Interstitial ad full screen content closed.");
            if (InterAdState != AdState.Loading) InterAdState = AdState.NotAvailable;
            ActionOnAfterInterstitalAd?.Invoke();
        };

        _interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.Log($"[{this.GetType().ToString()}] Interstitial ad failed to open full screen content with error : {error}");
            if (InterAdState != AdState.Loading) InterAdState = AdState.NotAvailable;
            ActionOnAfterInterstitalAd?.Invoke();
        };
    }
#endif
}
