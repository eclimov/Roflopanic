using System.Collections;
using System.Collections.Generic;
using UnityEngine.Localization;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public abstract class AbstractShopItem : MonoBehaviour
{
    protected ShopItemContainerData shopItemContainerData;
    protected Button button;

    private bool canBePurchased;
    protected virtual bool CanBePurchased
    {
        get
        {
            return canBePurchased;
        }

        set
        {
            canBePurchased = value;
            InitializeStyles();
        }
    }

    private WaitForSecondsRealtime cachedWaitForSecondsRealtimeGlintInterval;

    private GameObject confirmPurchasePanel;
    private ConfirmPurchasePanelManager confirmPurchasePanelManager;

    protected virtual void Awake()
    {
        shopItemContainerData = GetComponent<ShopItemContainerData>();
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);

        CheckIfCanBePurchased();
    }

    // Start is called before the first frame update
    void Start()
    {
        cachedWaitForSecondsRealtimeGlintInterval = new WaitForSecondsRealtime(Random.Range(5f, 8f));

        shopItemContainerData.itemData.SetItemSprite(shopItemContainerData.itemImage.sprite);
        shopItemContainerData.itemData.SetItemName(shopItemContainerData.itemNameText.text);

        shopItemContainerData.itemData.SetItemCurrencySprite(shopItemContainerData.itemCurrencyImage.sprite);
        shopItemContainerData.itemData.SetItemPriceValueText(shopItemContainerData.itemPriceValueText.text);

        // Start glint animation loop
        StartCoroutine(InfiniteGlintAnimation());

        SettingsManager.instance.OnTotalScoreChange += OnTotalScoreChange;
        SettingsManager.instance.OnEquippedItemsChange += InitializeStyles;
        SettingsManager.instance.OnPlayerSkinChange += InitializeStyles;
    }

    private IEnumerator InfiniteGlintAnimation()
    {
        while (true)
        {
            if(CanBePurchased && Random.Range(0, 5) == 0)
            {
                shopItemContainerData.glintAnimator.SetTrigger("Move");
            }

            yield return cachedWaitForSecondsRealtimeGlintInterval;
        }
    }

    private void OnTotalScoreChange(int newScore)
    {
        CheckIfCanBePurchased();
    }

    protected virtual void CheckIfCanBePurchased() // Has a different implementation for paid product(s)
    {
        CanBePurchased = !IsPurchased() && SettingsManager.GetTotalScore() >= shopItemContainerData.itemData.itemPrice;
    }

    protected void InitializeStyles()
    {
        if (IsPurchased())
        {
            shopItemContainerData.itemContainerImage.color = new Color32(109, 168, 0, 255);

            shopItemContainerData.itemPriceContainer.SetActive(false);

            if (CanBeEquipped())
            {
                shopItemContainerData.itemEquippedSprite.SetActive(true);

                if(IsEquipped())
                {
                    shopItemContainerData.itemEquippedSprite.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                } else
                {
                    shopItemContainerData.itemEquippedSprite.GetComponent<Image>().color = new Color32(0, 0, 0, 255);
                }
            }
        } else // Is not purchased
        {
            if(CanBePurchased)
            {
                shopItemContainerData.itemContainerImage.color = new Color32(255, 152, 23, 255);
            } else
            {
                shopItemContainerData.itemContainerImage.color = new Color32(150, 150, 150, 255);
            }
        }

        shopItemContainerData.itemContainerButton.interactable = CanBePurchased || CanBeEquipped();
    }

    public void OnClick()
    {
        if (CanBePurchased)
        {
            SpawnConfirmPurchasePanel();
        } else if(CanBeEquipped())
        { // Equip/unequip
            Equip();
            AudioManager.instance.PlayOpenBagSound();
        }
    }

    private void SpawnConfirmPurchasePanel()
    {
        ShopManager shopManager = FindObjectOfType<ShopManager>();
        confirmPurchasePanel = Instantiate(shopManager.confirmPurchasePanelPrefab, GameObject.Find("Canvas UI").transform, false);
        confirmPurchasePanelManager = confirmPurchasePanel.GetComponent<ConfirmPurchasePanelManager>();
        confirmPurchasePanelManager.SetData(shopItemContainerData.itemData);

        confirmPurchasePanelManager.OnCancel += DestroyConfirmPurchasePanel;
        confirmPurchasePanelManager.OnOk += PurchaseItem;
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(OnClick);

        if (confirmPurchasePanel != null)
        {
            confirmPurchasePanelManager.OnCancel -= DestroyConfirmPurchasePanel;
            confirmPurchasePanelManager.OnOk -= PurchaseItem;
        }

        SettingsManager.instance.OnTotalScoreChange -= OnTotalScoreChange;
        SettingsManager.instance.OnEquippedItemsChange -= InitializeStyles;
        SettingsManager.instance.OnPlayerSkinChange -= InitializeStyles;
    }

    protected void DestroyConfirmPurchasePanel()
    {
        Destroy(confirmPurchasePanel);
    }

    protected abstract void PurchaseItem();

    protected abstract bool IsPurchased();

    protected abstract bool CanBeEquipped();

    protected abstract bool IsEquipped();

    protected abstract void Equip();
}
