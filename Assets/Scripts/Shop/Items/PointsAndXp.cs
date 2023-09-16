using System.Collections;
using System.Collections.Generic;
using UnityEngine.Purchasing;
using UnityEngine;
using System;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine.Purchasing.Extension;

public class PointsAndXp : AbstractShopItem, IDetailedStoreListener
{
    [SerializeField]
    private GameObject itemPriceContainer;

    [SerializeField]
    private GameObject loadingSprite;

    private IStoreController StoreController;
    private IExtensionProvider ExtensionProvider;

    protected override bool CanBePurchased
    {
        get
        {
            return base.CanBePurchased;
        }

        set
        {
            base.CanBePurchased = value;

            loadingSprite.SetActive(!value);
            itemPriceContainer.SetActive(value);
        }
    }


    protected override async void Awake()
    {
        base.Awake();
        CanBePurchased = false; // By default. Do not remove, because setter should be called for side effects to trigger

        InitializationOptions options = new InitializationOptions()
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            .SetEnvironmentName("test");
#else
            .SetEnvironmentName("prod");
#endif

        await UnityServices.InitializeAsync(options);
        ResourceRequest operation = Resources.LoadAsync<TextAsset>("IAPProductCatalog");
        operation.completed += HandleIAPCatalogLoaded;
    }

    private void HandleIAPCatalogLoaded(AsyncOperation Operation)
    {
        ResourceRequest request = Operation as ResourceRequest;

        Debug.Log($"Loaded Asset: {request.asset}");
        ProductCatalog catalog = JsonUtility.FromJson<ProductCatalog>((request.asset as TextAsset).text);
        Debug.Log($"Loaded catalog with {catalog.allProducts.Count} items");

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        StandardPurchasingModule.Instance().useFakeStoreUIMode = FakeStoreUIMode.StandardUser;
        StandardPurchasingModule.Instance().useFakeStoreAlways = true;
#endif

#if UNITY_ANDROID
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(
            StandardPurchasingModule.Instance(AppStore.GooglePlay)
        );
#elif UNITY_IOS
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(
            StandardPurchasingModule.Instance(AppStore.AppleAppStore)
        );
#else
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(
            StandardPurchasingModule.Instance(AppStore.NotSpecified)
        );
#endif
        foreach (ProductCatalogItem item in catalog.allProducts)
        {
            builder.AddProduct(item.id, item.type);
        }

        UnityPurchasing.Initialize(this, builder);
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // The button remains disabled
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        OnInitializeFailed(error);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        // Purchase success
        // Debug.Log($"Successfully purchased {purchaseEvent.purchasedProduct.definition.id}");

        SettingsManager.instance.AddScore(10_000);
        SettingsManager.instance.AddExperience(10_000);
        AudioManager.instance.PlayCashSound();

        CanBePurchased = true;

        return PurchaseProcessingResult.Complete;
    }

    // Purchase fail
    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        // Obsolete. NOT BEING CALLED
        throw new System.NotImplementedException();
    }

    // Purchase fail
    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        CanBePurchased = true; // Allow trying purchasing again
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        StoreController = controller;
        ExtensionProvider = extensions;
        
        CanBePurchased = true;
    }


    //***************************************************************************************

    protected override void CheckIfCanBePurchased()
    {
        // Leave empty, because this product uses different logic for checking whether it can be purcased
    }

    protected override bool CanBeEquipped()
    {
        return false;
    }

    protected override bool IsEquipped()
    {
        return false;
    }

    protected override bool IsPurchased()
    {
        return false;
    }

    protected override void PurchaseItem()
    {
        DestroyConfirmPurchasePanel();

        CanBePurchased = false; // Prevent accidental double click
        StoreController.InitiatePurchase(shopItemContainerData.itemData.itemId);
    }

    protected override void Equip()
    {
        throw new NotImplementedException();
    }
}
