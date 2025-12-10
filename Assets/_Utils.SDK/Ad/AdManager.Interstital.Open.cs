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
    [Header("AD OPEN INTERSTITIAL")]
    public bool AlwayShowInterstitialAd = true;
    public bool InterstitialOpenAdShowing = false;
    public AdState InterstitialOpenAdState = AdState.NotAvailable;
    public string _adUnitInterOpenId = "ca-app-pub-8190506959251235/4942077087";

    private InterstitialAd _interstitialOpenAd;

    public void PromiseShowInterstitialOpenAd()
    {
        Debug.Log($"[{this.GetType().ToString()}] Promise show interstitial open Ad");
        var adRequest = new AdRequest();
        InterstitialAd.Load(_adUnitInterOpenId, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.Log($"[{this.GetType().ToString()}] interstitial ad failed to load an ad " +
                               "with error : " + error);
                return;
            }

            Debug.Log($"[{this.GetType().ToString()}] Interstitial ad loaded with response : "
                      + ad.GetResponseInfo());

            _interstitialOpenAd = ad;
            _interstitialOpenAd.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log(String.Format("Interstitial ad paid {0} {1}.",
                    adValue.Value,
                    adValue.CurrencyCode));
            };

            _interstitialOpenAd.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log($"[{this.GetType().ToString()}] Interstitial ad full screen content closed.");
                onCloseAd();
            };

            _interstitialOpenAd.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.Log($"[{this.GetType().ToString()}] Interstitial ad failed to open full screen content " +
                               "with error : " + error);
                onCloseAd();
            };

            _interstitialOpenAd.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log(String.Format("Interstitial ad paid {0} {1}.",
                    adValue.Value,
                    adValue.CurrencyCode));

                FirebaseManager.Instance.TRACKING_ADS_EVENT("paid_ads_interstitial", $"interstitial_open", (adValue.Value / (double)1000000).ToString());
                AppflyerEventSender.Instance.logAdRevenue(adValue);
            };

            Debug.Log($"[{this.GetType().ToString()}] Showing interstitial ad.");
            if (RuntimeStorageData.CanLoadAd() == true)
            {
                if (AlwayShowInterstitialAd == true)
                {
                    TaskRunner.EnqueueTask(() =>
                    {
                        onShowAd();
                        InterOpenAdMaximusTimeCounter = InterOpenAdMaximusTime;
                        _interstitialOpenAd.Show();
                    });
                }
                else
                {
                    TaskRunner.EnqueueTask(() =>
                    {
                        if (InterOpenAdMaximusTimeCounter <= InterOpenAdMaximusTime)
                        {
                            onShowAd();
                            InterOpenAdMaximusTimeCounter = InterOpenAdMaximusTime;
                            _interstitialOpenAd.Show();
                        }
                        else Debug.Log("Out of time show open ad");
                    });
                }
            }
        });
    }
#endif
}
