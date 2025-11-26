using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerSerializable
{
    public string Id;
    public bool IsAds;
    public int Gold;
    public string Language;
    public List<string> Packages = new();
    public int LastDayLogin;
    public int NumberOfDay;
    public bool IsFirstOpen;
    public int Level;

    public PlayerSerializable()
    {
        Debug.Log("Khởi tạo dữ liệu đầu tiên");
        Id = SystemInfo.deviceUniqueIdentifier;
        Gold = 0;
        Language = "English";
        Packages = new List<string>();
        LastDayLogin = 0;
        NumberOfDay = 0;
        IsFirstOpen = false;
        Level = 0;
        IsAds = false;
    }

    public bool CanLoadAd() =>
        Packages == null || !Packages.Contains(Config.REMOVE_AD_ID);

    public bool IsProductId(string productId) =>
        Packages != null && Packages.Contains(productId);

    public void AddProductId(string productId)
    {
        if (Packages != null && !Packages.Contains(productId))
            Packages.Add(productId);
    }
}

