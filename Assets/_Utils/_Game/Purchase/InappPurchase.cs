using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if IAPPURCHASE_ENABLE
using UnityEngine.Purchasing;
#endif

public class InappPurchase : SingletonNonMono<InappPurchase>
#if IAPPURCHASE_ENABLE
    , IDetailedStoreListener
#endif
{
    public InappProductList ProductList; // Kéo ScriptableObject vào đây qua Inspector hoặc Resources.Load

    public List<InappProduct> Products => ProductList != null ? ProductList.Products : new List<InappProduct>();

#if IAPPURCHASE_ENABLE
    private static IStoreController storeController;
    private static IExtensionProvider storeExtensionProvider;
#endif

    [Header("DEBUG"), SerializeField] private bool logDebug = false;
    private int lastPurchaseAttemptTimestamp = 0;

    public void InitializePurchasing()
    {
#if IAPPURCHASE_ENABLE
        if (storeController == null) InitializePurchasing();
        if (IsInitialized()) return;
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        foreach (var p in Products)
            builder.AddProduct(p.ProductId, p.Type);
        UnityPurchasing.Initialize(this, builder);
#endif
        Log("[InappPurchase] InitializePurchasing called");
    }

    public bool IsInitialized()
    {
#if IAPPURCHASE_ENABLE
        return storeController != null && storeExtensionProvider != null;
#else
        return false;
#endif
    }

    public bool HasProduct(string productId) => Products.Any(p => p.ProductId == productId);

    public string GetProductIdByIndex(int index) => (index >= 0 && index < Products.Count) ? Products[index].ProductId : "";

    public int GetProductIndexById(string id) => Products.FindIndex(p => p.ProductId == id);

    public string GetProductInfo(string productId)
    {
#if IAPPURCHASE_ENABLE
        if (!IsInitialized()) return GetLocalPrice(productId);
        var product = storeController.products.WithID(productId);
        return product?.metadata.localizedPriceString ?? GetLocalPrice(productId);
#else
        return GetLocalPrice(productId);
#endif
    }

    public string GetProductName(string productId) =>
        Products.FirstOrDefault(p => p.ProductId == productId)?.ProductName ?? "";

    private string GetLocalPrice(string productId) =>
        Products.FirstOrDefault(p => p.ProductId == productId)?.ProductPrice ?? "";

    private bool AntiSpamClick()
    {
        int now = UtilsTimerHelper.CurrentTimeInSeconds();
        if (now - lastPurchaseAttemptTimestamp > 1)
        {
            lastPurchaseAttemptTimestamp = now;
            return false;
        }
        return true;
    }

    public void BuyProduct(string productId, Action<bool> onComplete)
    {
#if IAPPURCHASE_ENABLE
        if (string.IsNullOrEmpty(productId) || AntiSpamClick()) return;
        Log($"[InappPurchase] Buy IAP: {productId}");
        currentProductId = productId;
        OnPurchaseComplete = complete =>
        {
            onComplete?.Invoke(complete);
            GameEvent.OnIAPurchaseMethod(productId);
        };
        if (IsInitialized())
        {
            var product = storeController.products.WithID(productId);
            if (product != null && product.availableToPurchase)
            {
                storeController.InitiatePurchase(product);
                Log($"[InappPurchase] Purchasing: {product.definition.id}");
            }
            else
            {
                Log("[InappPurchase] Product not available for purchase");
            }
        }
        else
        {
            Log("[InappPurchase] Not initialized, initializing now...");
            InitializePurchasing();
        }
#else
        onComplete?.Invoke(true);
#endif
    }

    public void RestorePurchases()
    {
#if IAPPURCHASE_ENABLE
        if (!IsInitialized())
        {
            Log("[InappPurchase] RestorePurchases FAIL. Not initialized.");
            return;
        }
        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
        {
            var apple = storeExtensionProvider.GetExtension<IAppleExtensions>();
            apple.RestoreTransactions((result, _) =>
            {
                Log("[InappPurchase] RestorePurchases result: " + result);
                if (result) RestoreProductIDs();
            });
        }
        else
        {
            Log("[InappPurchase] RestorePurchases not supported on this platform.");
        }
#endif
    }

    public void RestoreProductIDs()
    {
#if IAPPURCHASE_ENABLE
        if (!IsInitialized())
        {
            Log("[InappPurchase] RestoreProductIDs FAIL. Not initialized.");
            InitializePurchasing();
            return;
        }
        foreach (var data in Products)
        {
            var product = storeController.products.WithID(data.ProductId);
            if (product != null && product.hasReceipt)
            {
                RuntimeStorageData.Player.AddProductId(data.ProductId);
                GameEvent.OnIAPurchaseMethod(data.ProductId);
            }
        }
#endif
    }

    private void Log(string msg)
    {
        if (logDebug) Debug.Log(msg);
    }

#if IAPPURCHASE_ENABLE
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Log("[InappPurchase] OnInitialized: PASS");
        storeController = controller;
        storeExtensionProvider = extensions;
        RestoreProductIDs();
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Log("[InappPurchase] OnInitializeFailed: " + error);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        OnPurchaseComplete?.Invoke(currentProductId == args.purchasedProduct.definition.id);
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Log($"[InappPurchase] OnPurchaseFailed: {product.definition.storeSpecificId}, Reason: {failureReason}");
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Log($"[InappPurchase] OnInitializeFailed: {error}, {message}");
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        Log($"[InappPurchase] OnPurchaseFailed: {failureDescription.productId}, {failureDescription.reason}");
    }
#endif
}

[CreateAssetMenu(fileName = "InappProductList", menuName = "IAP/InappProductList")]
public class InappProductList : ScriptableObject
{
    public List<InappProduct> Products;
}

[Serializable]
public class InappProduct
{
    public int ID;
    public string ProductId;
    public string ProductName;
    public string ProductPrice;
#if IAPPURCHASE_ENABLE
    public UnityEngine.Purchasing.ProductType Type;
#endif
}