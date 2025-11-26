#if ADMOB
using DG.Tweening;
using GoogleMobileAds.Api;
//using PimDeWitte.UnityMainThreadDispatcher;

#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public partial class AdManager
{
#if ADMOB
    [Header("NATIVE OVERLAY AD")]
    public bool IsPreloadNativeOverlayAd = true;
    public AdState NativeOverlayAdState = AdState.NotAvailable;
    public int NativerOverlayAdReloadCount = 0;
    public int NativeOverlayShowCount = 0;

    public bool NativerOverlayAdUsed = false;
    public bool NativeOverlayShowing = false;

    //public float _WillOpenNativeOverlayAdAfter = 5.0f;
    private int _NumberOfNativeOverlay = 0;
    private float _OverlayAdTimer = 0;
    public float CollapseAdSpaceTimeCounter = 0.0f;
    public float CollapseAdSpaceTime = 15.0f;
    public string _adUnitNativerOverlayId = "ca-app-pub-3940256099942544/2247696110";

    private NativeOverlayAd _nativeOverlayAd;

    private void CaculaterCounterCollapseAd()
    {
        if (RuntimeStorageData.CanLoadAd() == false) return;
        if (Manager.Instance.IsLoading == true) return;
        if (IsCanAutoAd() == false) return;

        //if (IsCacheNativeFullScreen == true)
        {
            _OverlayAdTimer += Time.deltaTime;

            if (_NumberOfNativeOverlay == 0 && _OverlayAdTimer >= 15.0f)
            {
                _NumberOfNativeOverlay += 1;
                _OverlayAdTimer = 0.0f;
                ShowNativeOverlayAd();
            }
            else if(_NumberOfNativeOverlay >= 1 && _OverlayAdTimer >= 60.0f)
            {
                _NumberOfNativeOverlay += 1;
                _OverlayAdTimer = 0.0f;
                ShowNativeOverlayAd();
            }
        }

        if (NativeOverlayShowing == true)
        {
            CollapseAdSpaceTimeCounter += Time.deltaTime;
            if (CollapseAdSpaceTimeCounter >= 30.0f)
            {
                CollapseAdSpaceTimeCounter = 0;
                LoadNativeOverlayAd();
            }
        }
    }

    /// <summary>
    /// Loads the ad.
    /// </summary>
    public void LoadNativeOverlayAd()
    {
        if (RuntimeStorageData.CanLoadAd() == false) return;
        if (NativeOverlayAdState == AdState.Loading) return;
        NativeOverlayAdState = AdState.Loading;

        // Clean up the old ad before loading a new one.
        if (_nativeOverlayAd != null)
        {
            DestroyNativeOverlayAd();
        }

        Debug.Log($"[{this.GetType().ToString()}] Loading native overlay ad.");

        // Create a request used to load the ad.
        var adRequest = new AdRequest();

        // Optional: Define native ad options.
        var options = new NativeAdOptions();
        options.AdChoicesPlacement = AdChoicesPlacement.BottomRightCorner;
        options.MediaAspectRatio = MediaAspectRatio.Any;

        // Send the request to load the ad.
        NativeOverlayAd.Load(_adUnitNativerOverlayId, adRequest, options,
            (NativeOverlayAd ad, LoadAdError error) =>
            {
                if (error != null)
                {
                    Debug.LogError($"[{this.GetType().ToString()}] Native Overlay ad failed to load an ad " +
                               " with error: " + error);

                    NativeOverlayAdState = AdState.NotAvailable;
                    return;
                }

                // The ad should always be non-null if the error is null, but
                // double-check to avoid a crash.
                if (ad == null)
                {
                    Debug.LogError($"[{this.GetType().ToString()}] Unexpected error: Native Overlay ad load event " +
                               " fired with null ad and null error.");

                    NativeOverlayAdState = AdState.NotAvailable;
                    return;
                }

                // The operation completed successfully.
                Debug.Log($"[{this.GetType().ToString()}] Native Overlay ad loaded with response : " +
                       ad.GetResponseInfo());
                _nativeOverlayAd = ad;
                _nativeOverlayAd.OnAdClicked += onAdClicked;
                _nativeOverlayAd.OnAdPaid += (AdValue adValue) =>
                {
                    FirebaseManager.Instance.TRACKING_ADS_EVENT("paid_ads_native", "native_collapse", (adValue.Value / (double)1000000).ToString());
                    AppflyerEventSender.Instance.logAdRevenue(adValue);
                };

                NativeOverlayAdState = AdState.Ready;
                NativerOverlayAdUsed = false;

                if (NativeOverlayShowing == true) ShowNativeOverlayAd();
            });
    }

    private void onAdClicked()
    {
        TaskRunner.EnqueueTask(LoadNativeOverlayAd);
    }

    /// <summary>
    /// Renders the ad.
    /// </summary>
    private void RenderNativeOverlayAd()
    {
        if (_nativeOverlayAd != null)
        {
            Debug.Log($"[{this.GetType().ToString()}] Rendering Native Overlay ad.");

            var NativeOverlayAdCanvas = this.transform.FindChildByParent("NativeOverlayAdCanvas");
            var LoadingCanvas = NativeOverlayAdCanvas.FindChildByParent("LoadingCanvas");
            var ImageMainBackground = LoadingCanvas.GetComponent<Image>();
            ImageMainBackground.color = Color.white;
            //ImageMainBackground.color = Color.red;

            var overlayInterStyle = new NativeTemplateStyle();
            overlayInterStyle.TemplateId = NativeTemplateId.Medium;
            overlayInterStyle.MainBackgroundColor = Color.white;
            overlayInterStyle.CallToActionText = new NativeTemplateTextStyle();
            overlayInterStyle.CallToActionText.BackgroundColor = Color.blue;
            overlayInterStyle.CallToActionText.TextColor = Color.white;
            overlayInterStyle.CallToActionText.FontSize = 9;
            overlayInterStyle.CallToActionText.Style = NativeTemplateFontStyle.Bold;

            //int screenWidth = Screen.width;
            //int screenHeight = (int)(Screen.height * 0.4f);
            //var adSize = new AdSize(screenWidth, screenHeight);

            _nativeOverlayAd.RenderTemplate(overlayInterStyle, AdPosition.Bottom);
        }
    }

    /// <summary>
    /// Shows the ad.
    /// </summary>
    public void ShowNativeOverlayAd()
    {
        if (RuntimeStorageData.CanLoadAd() == false) return;
        if (_nativeOverlayAd != null)
        {
            Debug.Log($"[{this.GetType().ToString()}] Showing Native Overlay ad.");

            NativeOverlayShowCount += 1;

            var NativeOverlayAdCanvas = this.transform.FindChildByParent("NativeOverlayAdCanvas");
            NativeOverlayAdCanvas.Show();
            RenderNativeOverlayAd();
            DOVirtual.DelayedCall(0.1f, _nativeOverlayAd.Show);

            NativerOverlayAdUsed = true;
            NativeOverlayShowing = true;

            HideBanner();
        }
    }

    /// <summary>
    /// Hides the ad.
    /// </summary>
    public void HideNativeOverlayAd()
    {
        if (_nativeOverlayAd != null)
        {
            _nativeOverlayAd.Hide();
        }
        Debug.Log($"[{this.GetType().ToString()}] Hiding Native Overlay ad.");
        var NativeOverlayAdCanvas = this.transform.FindChildByParent("NativeOverlayAdCanvas");
        NativeOverlayAdCanvas.Hide();

        NativeOverlayAdState = AdState.NotAvailable;
        NativeOverlayShowing = false;

        ShowBanner();
    }

    /// <summary>
    /// Destroys the native overlay ad.
    /// </summary>
    public void DestroyNativeOverlayAd()
    {
        if (_nativeOverlayAd != null)
        {
            Debug.Log($"[{this.GetType().ToString()}] Destroying native overlay ad.");
            _nativeOverlayAd.Destroy();
            _nativeOverlayAd = null;
        }
    }
#else
    public void ShowAdNativeOverlay() { }
#endif
}
