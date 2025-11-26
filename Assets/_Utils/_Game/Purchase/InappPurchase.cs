using System;
using System.Collections.Generic;
using UnityEngine;
#if IAPPURCHASE_ENABLE
using UnityEngine.Purchasing;


public class InappPurchase : Singleton<InappPurchase>, IDetailedStoreListener
#else
public class InappPurchase : Singleton<InappPurchase>
#endif
{
    public event Action<bool> OnPurchaseComplete;

#if IAPPURCHASE_ENABLE
    public static InappController Instance = null;
    private static IStoreController m_StoreController;
    private static IExtensionProvider m_StoreExtensionProvider;
#endif

    private string currentProduct;

    public List<InappProduct> products;

    [Space, Header("DEBUG"), SerializeField]
    private bool logDebug = false;

    private void Start()
    {
        if (logDebug)
            Debug.Log("[InappPurchase] initializing");
#if IAPPURCHASE_ENABLE
        if (m_StoreController == null)
        {
            InitializePurchasing();
            if (logDebug)
                Debug.Log("[InappPurchase] initialized");
        }
#endif
    }

    public void InitializePurchasing()
    {
        if (IsInitialized())
        {
            return;
        }

#if IAPPURCHASE_ENABLE
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        //add product
        foreach (InappProduct product in products)
        {
            builder.AddProduct(product.productId, product.type);
        }

        UnityPurchasing.Initialize(this, builder);
#endif
        if (logDebug)
            Debug.Log("[InappPurchase] Initialize Purchasing");
    }


    public bool IsInitialized()
    {
#if IAPPURCHASE_ENABLE
        return m_StoreController != null && m_StoreExtensionProvider != null;
#else
        return false;
#endif
    }

    public bool IsProductId(string productId)
    {
        for (int i = 0; i < products.Count; i++)
        {
            if (products[i].productId == productId)
                return true;
        }
        return false;
    }    

    public string GetProductIdByIndex(int _index)
    {
        if (products.Count <= _index) return "";
        return products[_index].productId;
    }

    public int GetProductIndexById(string _id)
    {
        for(int i = 0; i < products.Count; i++)
        {
            if (products[i].productId == _id)
                return products[i].ID;
        }

        return -1;
    }

    public string GetProductInfo(string productId)
    {
#if IAPPURCHASE_ENABLE
        if (IsInitialized() == false)
            return getProductLocalPrice(productId);
        var product = m_StoreController.products.WithID(productId);
        if (product != null)
        {
            return product.metadata.localizedPriceString;
        }
#endif
        return getProductLocalPrice(productId);
    }

    public string GetProductName(string productId)
    {
        for (int i = 0; i < products.Count; i++)
        {
            if (products[i].productId == productId)
                return products[i].productName;
        }
        return "";
    }

    private string getProductLocalPrice(string productId)
    {
        for (int i = 0; i < products.Count; i++)
        {
            if (products[i].productId == productId)
                return products[i].productPrice;
        }
        return "";
    }    

    int lastTimeClickBuyInappurchase = 0;
    private bool AntiSpamClick()
    {
        if (UtilsTimerHelper.CurrentTimeInSeconds() - lastTimeClickBuyInappurchase > 1.0)
        {
            lastTimeClickBuyInappurchase = UtilsTimerHelper.CurrentTimeInSeconds();
            return false;
        }
        return true;
    }    

    public void BuyProductID(string productId, Action<bool> _OnPurchaseComplete)
    {
#if IAPPURCHASE_ENABLE
        if(string.IsNullOrEmpty(productId)) return;
        if (AntiSpamClick() == true) return;
        var index = GetProductIndexById(productId);
        Debug.Log($"[InappPurchase] Buy IAP: {productId} | index: {index}");
        AdManager.Instance.OpenAdSpaceTimeCounter = 0;
        currentProduct = productId;
        if (IsInitialized())
        {
            Product product = m_StoreController.products.WithID(productId);
            if (product != null && product.availableToPurchase)
            {
                OnPurchaseComplete = (complete) =>
                {
                    string price = product.metadata.localizedPriceString;
                    string currencySymbol = product.metadata.isoCurrencyCode;
                    string currencyCode = product.metadata.isoCurrencyCode;

                    string isSubscription = product.definition.type == ProductType.Subscription ? "true" : "false";

                    Debug.Log("[InappPurchase] Product price: " + price);
                    Debug.Log("[InappPurchase] Currency symbol: " + currencySymbol);
                    Debug.Log("[InappPurchase] Currency code: " + currencyCode);

                    _OnPurchaseComplete?.Invoke(complete);
                    GameEvent.OnIAPurchaseMethod(productId);
                };
                if (logDebug)
                    Debug.Log(string.Format("[InappPurchase] Purchasing product asychronously: '{0}'", product.definition.id));
                m_StoreController.InitiatePurchase(product);
            }
            else
            {
                if (logDebug)
                    Debug.Log("[InappPurchase] BuyProductID: FAIL. Not purchasing product" +
                        ", either is not found or is not available for purchase");
            }
        }
        else
        {
            if (logDebug)
                Debug.Log("[InappPurchase] BuyProductID FAIL. Not initialized.");

            InitializePurchasing();
        }
#else
        _OnPurchaseComplete?.Invoke(true);
#endif
    }

    public void RestorePurchases()
    {
        if (!IsInitialized())
        {
            if (logDebug)
                Debug.Log("[InappPurchase] RestorePurchases FAIL. Not initialized.");
            return;
        }
#if IAPPURCHASE_ENABLE
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            if (logDebug)
                Debug.Log("[InappPurchase] RestorePurchases started ...");

            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            apple.RestoreTransactions((result, _str) =>
            {
                if (logDebug)
                    Debug.Log("[InappPurchase] RestorePurchases continuing: " + result
                        + ". If no further messages, no purchases available to restore.");

                if (result) RestoreProductID();
            });
        }
        else
        {
            if (logDebug)
                Debug.Log("[InappPurchase] RestorePurchases FAIL. Not supported on this platform. Current = "
                    + Application.platform);
        }
#endif
    }

    public void RestoreProductID()
    {
#if IAPPURCHASE_ENABLE
        if (IsInitialized())
        {
            foreach (InappProduct data in products)
            {
                Product product = m_StoreController.products.WithID(data.productId);
                if (product != null)
                {
                    if (product.hasReceipt) GameEvent.OnIAPurchaseMethod(data.productId);
                }
            }
        }
        else
        {
            Debug.Log("[InappPurchase] RestoreProductID FAIL. Not initialized.");
            InitializePurchasing();
        }
#endif
    }
 
#if IAPPURCHASE_ENABLE
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        if (logDebug)
            Debug.Log("[InappPurchase] OnInitialized: PASS");
        m_StoreController = controller;
        m_StoreExtensionProvider = extensions;

        Debug.Log("[InappPurchase] OnInitialized: Restorepurchase");
        foreach (InappProduct data in products)
        {
            Product product = m_StoreController.products.WithID(data.productId);
            if (product != null)
            {
                if (product.hasReceipt == true)
                {
                    RuntimeStorageData.Player.AddProductId(data.productId);
                    GameEvent.OnIAPurchaseMethod(data.productId);
                }
            }
            Debug.Log($"[InappPurchase] {product.hasReceipt} | {data.productId}");
        }
    }   


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        if (logDebug)
            Debug.Log("[InappPurchase] OnInitializeFailed InitializationFailureReason:" + error);
    }


    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        OnPurchaseComplete?.Invoke(currentProduct == args.purchasedProduct.definition.id);
        return PurchaseProcessingResult.Complete;
    }


    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        if (logDebug)
            Debug.Log(string.Format("[InappPurchase] OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}",
                product.definition.storeSpecificId, failureReason));
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        throw new NotImplementedException();
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        throw new NotImplementedException();
    }

#endif
}

[Serializable]
public class InappProduct
{
    public int ID;
    public string productId;
    public string productName;
    public string productPrice;
#if IAPPURCHASE_ENABLE
    public ProductType type;
#endif
}