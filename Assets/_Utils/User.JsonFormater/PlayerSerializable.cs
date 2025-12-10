using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerSerializable
{
    public string PlayerId;
    public bool IsAdsRemoved;
    public int GoldAmount;
    public string PreferredLanguage;
    public List<string> OwnedPackages;
    public int LastLoginDay;
    public int TotalLoginDays;
    public bool IsFirstLaunch;
    public int CurrentLevel;
    public string ExtensionData;

    public PlayerSerializable()
    {
        PlayerId = SystemInfo.deviceUniqueIdentifier;
        GoldAmount = 0;
        PreferredLanguage = "English";
        OwnedPackages = new List<string>();
        LastLoginDay = 0;
        TotalLoginDays = 0;
        IsFirstLaunch = false;
        CurrentLevel = 0;
        IsAdsRemoved = false;
        ExtensionData = "{}";
    }

    public bool CanShowAds() =>
        OwnedPackages == null || !OwnedPackages.Contains(Config.REMOVE_AD_ID);

    public bool HasProduct(string productId) =>
        OwnedPackages != null && OwnedPackages.Contains(productId);

    public void AddProduct(string productId)
    {
        if (OwnedPackages != null && !OwnedPackages.Contains(productId))
            OwnedPackages.Add(productId);
    }
}

